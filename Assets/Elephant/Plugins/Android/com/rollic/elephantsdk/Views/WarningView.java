package com.rollic.elephantsdk.Views;

import android.content.Context;
import android.view.Gravity;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.rollic.elephantsdk.Models.WarningViewModel;
import com.rollic.elephantsdk.Utils.Utils;

public class WarningView extends LinearLayout {

    TextView warningTextView;

    public WarningView(Context ctx) {
        super(ctx);

        setupLayout();
        setupWarningTextView();
    }

    private void setupLayout() {
        setLayoutParams(new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                ViewGroup.LayoutParams.WRAP_CONTENT
        ));
        setGravity(Gravity.CENTER);
        setPadding(0, 0, 0, Utils.dpToPx(15));
    }

    private void setupWarningTextView() {
        warningTextView = new TextView(getContext());
        warningTextView.setLayoutParams(new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                ViewGroup.LayoutParams.WRAP_CONTENT
        ));
        warningTextView.setTextSize(10.0f);

        addView(warningTextView);
    }

    public void configure(WarningViewModel model) {
        configureWarningTextView(model.content);
    }

    private void configureWarningTextView(String content) {
        this.warningTextView.setText(content);
    }
}
