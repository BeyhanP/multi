package com.rollic.elephantsdk.Views;

import android.content.Context;
import android.view.Gravity;
import android.widget.LinearLayout;
import android.widget.ProgressBar;

import com.rollic.elephantsdk.Utils.Utils;

public class LoadingView extends LinearLayout {

    public ProgressBar progressBar;

    public LoadingView(Context context) {
        super(context);

        setupLayout();
        setupProgressBar();
    }

    private void setupLayout() {
        setGravity(Gravity.CENTER);
    }

    private void setupProgressBar() {
        progressBar = new ProgressBar(getContext());
        progressBar.setLayoutParams(new LayoutParams(Utils.dpToPx(20), Utils.dpToPx(20)));
        progressBar.setVisibility(VISIBLE);

        addView(progressBar);
    }
}
