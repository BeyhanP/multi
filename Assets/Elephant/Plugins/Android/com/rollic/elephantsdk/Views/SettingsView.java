package com.rollic.elephantsdk.Views;

import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.net.Uri;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;

import android.widget.FrameLayout;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;

import com.rollic.elephantsdk.Interaction.InteractionType;
import com.rollic.elephantsdk.Models.ActionType;
import com.rollic.elephantsdk.Models.ComplianceAction;
import com.rollic.elephantsdk.Models.DialogModels.PersonalizedAdsDialogModel;
import com.rollic.elephantsdk.Models.DialogModels.SettingsDialogModel;
import com.rollic.elephantsdk.Models.DialogSubviewType;
import com.rollic.elephantsdk.Payload.URLPayload;
import com.rollic.elephantsdk.Models.RollicButton;
import com.rollic.elephantsdk.Utils.Utils;

public class SettingsView extends BaseDialog<SettingsDialogModel> {

    public static SettingsView instance;

    FrameLayout titleLayout;
    TextView titleTextView;
    RollicButton closeButton;
    LinearLayout complianceActionButtonsLayout;

    LinearLayout.LayoutParams textViewLayoutParams;
    LinearLayout.LayoutParams buttonLayoutParams;

    public SettingsView(Context context) {
        super(context);

        setupTitleLayout();
        setupChildViewLayoutParams();
        setupTitleTextView();
        setupCloseButton();
        setupComplianceActionButtonsLayout();
    }

    @Override
    protected void setupContentView() {
        super.setupContentView();

        contentView.setPadding(
                Utils.dpToPx(15),
                Utils.dpToPx(20),
                Utils.dpToPx(15),
                Utils.dpToPx(20)
        );
    }

    private void setupTitleLayout() {
        titleLayout = new FrameLayout(getContext());
        titleLayout.setLayoutParams(new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                Utils.dpToPx(60)
        ));
        titleLayout.setPadding(
                Utils.dpToPx(5),
                Utils.dpToPx(0),
                Utils.dpToPx(5),
                Utils.dpToPx(0)
        );

        contentView.addView(titleLayout);
    }

    private void setupChildViewLayoutParams() {
        textViewLayoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.WRAP_CONTENT);
        textViewLayoutParams.weight = 1;
        textViewLayoutParams.setMargins(0, 0, 0, Utils.dpToPx(10));

        buttonLayoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.WRAP_CONTENT);
        buttonLayoutParams.setMargins(0, Utils.dpToPx(10), 0, 0);
    }

    protected void setupTitleTextView() {
        titleTextView = new TextView(getContext());
        titleTextView.setText("Settings");
        titleTextView.setTextAlignment(View.TEXT_ALIGNMENT_CENTER);
        titleTextView.setTextSize(25.0f);
        titleTextView.setLayoutParams(new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                ViewGroup.LayoutParams.MATCH_PARENT
        ));
        titleTextView.setGravity(Gravity.CENTER_VERTICAL);
        titleLayout.addView(titleTextView);
    }

    private void setupCloseButton() {
        FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.WRAP_CONTENT,
                ViewGroup.LayoutParams.MATCH_PARENT
        );

        layoutParams.gravity = Gravity.RIGHT;
        closeButton = new RollicButton(getContext(), "X");
        closeButton.setTextSize(20.0f);
        closeButton.setBackgroundColor(Color.TRANSPARENT);
        closeButton.setTextAlignment(View.TEXT_ALIGNMENT_VIEW_END);
        closeButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                OnButtonClicked(v, true);
            }
        });

        titleLayout.addView(closeButton, layoutParams);
    }

    private void setupComplianceActionButtonsLayout() {
        complianceActionButtonsLayout = new LinearLayout(getContext());
        complianceActionButtonsLayout.setOrientation(LinearLayout.VERTICAL);
        complianceActionButtonsLayout.setLayoutParams(new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                ViewGroup.LayoutParams.WRAP_CONTENT
        ));

        contentView.addView(complianceActionButtonsLayout);
    }

    @Override
    public void configureWithModel(SettingsDialogModel model) {
        super.configureWithModel(model);

        setComplianceActionButtons(model.complianceActions);
    }

    @Override
    protected void OnButtonClicked(View v, boolean shouldDismissDialog) {
        super.OnButtonClicked(v, shouldDismissDialog);

        RollicButton button = (RollicButton) v;
        ComplianceAction complianceAction = button.complianceAction;

        try {
            switch (complianceAction.action) {
                case URL:
                    URLPayload urlPayload = complianceAction.getPayload();
                    Intent viewIntent =  new Intent(
                            "android.intent.action.VIEW",
                            Uri.parse(urlPayload.url));
                    getContext().startActivity(viewIntent);
                    break;
                case DATA_REQUEST:
                    interactionInterface.OnButtonClick(InteractionType.CALL_DATA_REQUEST);
                    break;
                case CCPA: case GDPR_AD_CONSENT:
                    PersonalizedAdsConsentView personalizedAdsConsentView =
                            new PersonalizedAdsConsentView(getContext());

                    personalizedAdsConsentView.configureWithComplianceAction(interactionInterface, complianceAction);

                    personalizedAdsConsentView.show(DialogSubviewType.CONTENT);
                    break;
                case CUSTOM_POPUP:
                    break;
            }
        } catch (NullPointerException e) {
            e.printStackTrace();
        }
    }

    private void setComplianceActionButtons(@NonNull ComplianceAction[] complianceActions) {
        removeAllComplianceActionButtons();

        for(ComplianceAction complianceAction: complianceActions) {
            addComplianceActionButton(complianceAction);
        }
    }

    private void addComplianceActionButton(ComplianceAction model) {
        RollicButton button = new RollicButton(getContext(), model);

        button.setMinHeight(Utils.dpToPx(60));
        button.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                OnButtonClicked(v, model.action == ActionType.DATA_REQUEST);
            }
        });

        complianceActionButtonsLayout.addView(button, buttonLayoutParams);
    }

    private void removeAllComplianceActionButtons() {
        complianceActionButtonsLayout.removeAllViewsInLayout();
    }

    public static SettingsView newInstance(Context ctx) {
        if (instance == null) {
            instance = new SettingsView(ctx);
        }

        return instance;
    }
}
