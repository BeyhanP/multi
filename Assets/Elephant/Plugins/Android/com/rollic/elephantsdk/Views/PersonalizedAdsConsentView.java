package com.rollic.elephantsdk.Views;

import android.content.Context;
import android.graphics.Color;
import android.text.method.LinkMovementMethod;
import android.util.Log;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.Space;
import android.widget.TextView;

import com.rollic.elephantsdk.Hyperlink.Hyperlink;
import com.rollic.elephantsdk.Interaction.InteractionInterface;
import com.rollic.elephantsdk.Interaction.InteractionType;
import com.rollic.elephantsdk.Models.ActionType;
import com.rollic.elephantsdk.Models.ComplianceAction;
import com.rollic.elephantsdk.Models.DialogModels.PersonalizedAdsDialogModel;
import com.rollic.elephantsdk.Models.RollicButton;
import com.rollic.elephantsdk.Utils.StringUtils;
import com.rollic.elephantsdk.Utils.Utils;

public class PersonalizedAdsConsentView extends BaseDialog<PersonalizedAdsDialogModel> {

    public static PersonalizedAdsConsentView instance;

    TextView titleTextView;
    TextView contentTextView;
    LinearLayout consentActionButtonsLayout;
    RollicButton declineButton;
    RollicButton agreeButton;
    RollicButton backToGameButton;

    ActionType action;

    public PersonalizedAdsConsentView(Context ctx) {
        super(ctx);

        setupTitleTextView();
        setupContentTextView();
        setupConsentActionButtonsLayout();
        setupDeclineButton();
        setupDivider();
        setupAgreeButton();
        setupBackToGameButton();
    }

    private void setupTitleTextView() {
        LinearLayout.LayoutParams textViewLayoutParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                ViewGroup.LayoutParams.WRAP_CONTENT);
        textViewLayoutParams.setMargins(0, 0, 0, Utils.dpToPx(10));

        titleTextView = new TextView(getContext());
        titleTextView.setTextColor(Color.WHITE);
        titleTextView.setTextAlignment(View.TEXT_ALIGNMENT_CENTER);
        titleTextView.setTextSize(20.0f);
        titleTextView.setSingleLine();

        contentView.addView(titleTextView, textViewLayoutParams);
    }

    private void setupContentTextView() {
        LinearLayout.LayoutParams textViewLayoutParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                ViewGroup.LayoutParams.WRAP_CONTENT);

        textViewLayoutParams.setMargins(0, 0, 0, Utils.dpToPx(10));

        contentTextView = new TextView(getContext());
        contentTextView.setTextColor(Color.WHITE);
        contentTextView.setTextSize(15.0f);
        contentTextView.setLinksClickable(true);
        contentTextView.setClickable(true);
        contentTextView.setTextIsSelectable(true);
        contentTextView.setMovementMethod(LinkMovementMethod.getInstance());

        contentView.addView(contentTextView, textViewLayoutParams);
    }

    private void setupConsentActionButtonsLayout() {
        consentActionButtonsLayout = new LinearLayout(getContext());
        consentActionButtonsLayout.setOrientation(LinearLayout.HORIZONTAL);
        consentActionButtonsLayout.setLayoutParams(new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                Utils.dpToPx(65)));
        consentActionButtonsLayout.setWeightSum(2);
        consentActionButtonsLayout.setPadding(0, 0, 0, Utils.dpToPx(5));

        contentView.addView(consentActionButtonsLayout);
    }

    private void setupDeclineButton() {
        LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                ViewGroup.LayoutParams.MATCH_PARENT);
        layoutParams.weight = 1;

        declineButton = new RollicButton(getContext());
        declineButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                OnButtonClicked(v, true);
            }
        });

        consentActionButtonsLayout.addView(declineButton, layoutParams);
    }

    private void setupDivider() {
        Space divider = new Space(getContext());
        divider.setLayoutParams(new LinearLayout.LayoutParams(
                Utils.dpToPx(5),
                ViewGroup.LayoutParams.MATCH_PARENT
        ));

        consentActionButtonsLayout.addView(divider);
    }

    private void setupAgreeButton() {
        LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                ViewGroup.LayoutParams.MATCH_PARENT);
        layoutParams.weight = 1;

        agreeButton = new RollicButton(getContext());
        agreeButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                OnButtonClicked(v, true);
            }
        });

        consentActionButtonsLayout.addView(agreeButton, layoutParams);
    }

    private void setupBackToGameButton() {
        backToGameButton = new RollicButton(getContext());
        backToGameButton.setLayoutParams(new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                Utils.dpToPx(60)));
        backToGameButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                OnButtonClicked(v, true);
            }
        });

        contentView.addView(backToGameButton);
    }

    @Override
    public void configureWithModel(PersonalizedAdsDialogModel model) {
        super.configureWithModel(model);

        this.action = model.action;
        configureTitleTextView(model.title);
        configureContentTextView(model.content, model.hyperlinks);
        configureDeclineButton(model.declineButtonTitle);
        configureAgreeButton(model.agreeButtonTitle);
        configureBackToGameButton(model.actionButtonTitle);
    }

    public void configureWithComplianceAction(InteractionInterface interactionInterface, ComplianceAction complianceAction) {
        PersonalizedAdsDialogModel model = new PersonalizedAdsDialogModel(interactionInterface, complianceAction);

        super.configureWithModel(model);

        this.action = model.action;
        configureTitleTextView(model.title);
        configureContentTextView(model.content, model.hyperlinks);
        configureDeclineButton(model.declineButtonTitle);
        configureAgreeButton(model.agreeButtonTitle);
        configureBackToGameButton(model.actionButtonTitle);
    }

    private void configureTitleTextView(String title) {
        titleTextView.setText(title);
    }

    private void configureContentTextView(String content, Hyperlink[] hyperlinks) {
        contentTextView.setText(StringUtils.configurePopUpHtmlContent(content, hyperlinks));
    }

    private void configureDeclineButton(String title) {
        declineButton.setText(title);
    }

    private void configureAgreeButton(String title) {
        agreeButton.setText(title);
    }

    private void configureBackToGameButton(String title) {
        backToGameButton.setText(title);
    }

    @Override
    protected void OnButtonClicked(View v, boolean shouldDismissDialog) {
        super.OnButtonClicked(v, shouldDismissDialog);

        RollicButton button = (RollicButton) v;

        switch (action) {
            case CCPA:
                if(button == agreeButton) {
                    interactionInterface.OnButtonClick(InteractionType.PERSONALIZED_ADS_AGREE);
                } else if(button == declineButton) {
                    interactionInterface.OnButtonClick(InteractionType.PERSONALIZED_ADS_DECLINE);
                }
                break;
            case GDPR_AD_CONSENT:
                if(button == agreeButton) {
                    interactionInterface.OnButtonClick(InteractionType.GDPR_AD_CONSENT_AGREE);
                } else if(button == declineButton) {
                    interactionInterface.OnButtonClick(InteractionType.GDPR_AD_CONSENT_DECLINE);
                }
                break;
            default:
                break;
        }

    }

    public static PersonalizedAdsConsentView newInstance(Context ctx) {
        if (instance == null) {
            instance = new PersonalizedAdsConsentView(ctx);
        }

        return instance;
    }

    @Override
    public void onWindowFocusChanged(boolean hasFocus) {
        super.onWindowFocusChanged(hasFocus);

        int dialogHeight = this.getWindow().getDecorView().getHeight();
        int screenHeight = Utils.screenHeight();

        if (dialogHeight > (double)screenHeight * 0.9) {
            int maxHeight = (int) ((double) Utils.screenHeight() * 0.45);

            contentTextView.setMaxHeight(maxHeight);
        }
    }
}
