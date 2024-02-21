package com.rollic.elephantsdk.Views;

import android.content.Context;
import android.graphics.Color;
import android.text.method.LinkMovementMethod;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.rollic.elephantsdk.Hyperlink.Hyperlink;
import com.rollic.elephantsdk.Models.DialogModels.GenericDialogModel;
import com.rollic.elephantsdk.Models.RollicButton;
import com.rollic.elephantsdk.Utils.StringUtils;
import com.rollic.elephantsdk.Utils.Utils;

public class GenericDialog<T extends GenericDialogModel> extends BaseDialog<T> {

    public interface ButtonActionHandler {
        void onButtonClickHandler();
    }

    public static GenericDialog instance;

    TextView titleTextView;
    TextView contentTextView;
    RollicButton actionButton;

    private ButtonActionHandler buttonActionHandler;

    public GenericDialog(Context ctx) {
        super(ctx);

        setupTitleTextView();
        setupContentTextView();
        setupActionButton();
    }

    protected void setupTitleTextView() {
        LinearLayout.LayoutParams textViewLayoutParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                ViewGroup.LayoutParams.WRAP_CONTENT);
        textViewLayoutParams.weight = 1;
        textViewLayoutParams.setMargins(0, 0, 0, Utils.dpToPx(10));

        titleTextView = new TextView(getContext());
        titleTextView.setTextColor(Color.WHITE);
        titleTextView.setTextAlignment(View.TEXT_ALIGNMENT_CENTER);
        titleTextView.setTextSize(20.0f);

        contentView.addView(titleTextView, textViewLayoutParams);
    }

    protected void setupContentTextView() {
        LinearLayout.LayoutParams textViewLayoutParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                ViewGroup.LayoutParams.WRAP_CONTENT);
        textViewLayoutParams.weight = 1;
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

    protected void setupActionButton() {
        actionButton = new RollicButton(getContext());
        actionButton.setTextSize(18.0f);
        actionButton.setLayoutParams(new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                Utils.dpToPx(60)));
        actionButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                OnButtonClicked(v, true);
            }
        });

        contentView.addView(actionButton);
    }

    @Override
    public void configureWithModel(T model) {
        super.configureWithModel(model);

        configureTitleTextView(model.title);
        configureContentTextView(model.content, model.hyperlinks);
        configureActionButton(model.actionButtonTitle);
    }

    public void configureButtonActionHandler(ButtonActionHandler buttonActionHandler) {
        this.buttonActionHandler = buttonActionHandler;
    }

    private void configureTitleTextView(String title) {
        int visibility = title.isEmpty() ? View.GONE : View.VISIBLE;

        titleTextView.setVisibility(visibility);
        titleTextView.setText(title);
    }

    private void configureContentTextView(String content, Hyperlink[] hyperlinks) {
        contentTextView.setText(StringUtils.configurePopUpHtmlContent(content, hyperlinks));
    }

    private void configureActionButton(String buttonTitle) {
        actionButton.setText(buttonTitle);
    }

    public static GenericDialog newInstance(Context ctx) {
        if (instance == null) {
            instance = new GenericDialog(ctx);
        }

        return instance;
    }

    @Override
    protected void OnButtonClicked(View v, boolean shouldDismissDialog) {
        super.OnButtonClicked(v, shouldDismissDialog);

        if (buttonActionHandler != null) {
            buttonActionHandler.onButtonClickHandler();
        }
    }
}
