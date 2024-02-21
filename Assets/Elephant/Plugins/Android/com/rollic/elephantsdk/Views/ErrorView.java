package com.rollic.elephantsdk.Views;

import android.content.Context;
import android.graphics.Color;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.rollic.elephantsdk.Models.ErrorViewModel;
import com.rollic.elephantsdk.Models.RollicButton;
import com.rollic.elephantsdk.Utils.Utils;

public class ErrorView extends LinearLayout {

    TextView messageTextView;
    RollicButton okButton;

    public ErrorView(Context ctx) {
        super(ctx);

        setupLayout();
        setupMessageTextView();
        setupOkButton();
    }

    private void setupLayout() {
        setOrientation(VERTICAL);

    }

    private void setupMessageTextView() {
        LayoutParams layoutParams = new LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                ViewGroup.LayoutParams.WRAP_CONTENT
        );
        layoutParams.setMargins(0, 0, 0, Utils.dpToPx(20));

        messageTextView = new TextView(getContext());

        messageTextView.setTextAlignment(TEXT_ALIGNMENT_CENTER);
        messageTextView.setTextColor(Color.WHITE);
        messageTextView.setTextSize(15.0f);

        addView(messageTextView, layoutParams);
    }

    private void setupOkButton() {
        okButton = new RollicButton(getContext(), "OK");

        okButton.setTextSize(18.0f);
        okButton.setLayoutParams(new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                Utils.dpToPx(60)));

        addView(okButton);
    }

    public void configure(ErrorViewModel model) {
        configureMessageTextView(model.message);
    }

    private void configureMessageTextView(String message) {
        messageTextView.setText(message);
    }

    public void setOkButtonOnClickListener(OnClickListener l) {
        okButton.setOnClickListener(l);
    }

    @Override
    public void setLayoutParams(ViewGroup.LayoutParams params) {
        super.setLayoutParams(params);

        if (params instanceof LayoutParams) {
            ((LayoutParams) params).setMargins(
                    Utils.dpToPx(10),
                    Utils.dpToPx(20),
                    Utils.dpToPx(10),
                    Utils.dpToPx(15)
            );
        }
    }
}