package com.rollic.elephantsdk.Views;

import android.app.Dialog;
import android.content.Context;
import android.util.Log;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.widget.LinearLayout;

import androidx.annotation.NonNull;

import com.rollic.elephantsdk.Interaction.InteractionInterface;
import com.rollic.elephantsdk.Models.DialogModels.BaseDialogModel;
import com.rollic.elephantsdk.Models.DialogSubviewType;
import com.rollic.elephantsdk.Models.ErrorViewModel;
import com.rollic.elephantsdk.Utils.Utils;

public class BaseDialog<DialogModel extends BaseDialogModel> extends Dialog {

    public static BaseDialog instance;

    LinearLayout parentLayout;
    LoadingView loadingView;
    ErrorView errorView;
    LinearLayout contentView;

    public InteractionInterface interactionInterface;

    int width = (int) (((double) Utils.screenWidth())*0.8);

    public BaseDialog(Context ctx) {
        super(ctx, android.R.style.Theme_DeviceDefault_Dialog);

        setupDialog();
        setupLayout();
        setupLoadingView();
        setupErrorView();
        setupContentView();
    }

    protected void setupDialog() {
        setCancelable(false);
        requestWindowFeature(Window.FEATURE_NO_TITLE);
    }

    protected void setupLayout() {
        parentLayout = new LinearLayout(getContext());

        parentLayout.setOrientation(LinearLayout.VERTICAL);
        parentLayout.setLayoutParams(new LinearLayout.LayoutParams(
                width,
                ViewGroup.LayoutParams.WRAP_CONTENT)
        );
        parentLayout.setGravity(Gravity.CENTER);

        setContentView(parentLayout);
    }

    protected void setupLoadingView() {
        loadingView = new LoadingView(getContext());

        loadingView.setLayoutParams(new LinearLayout.LayoutParams(
                width,
                Utils.dpToPx(300)
        ));

        parentLayout.addView(loadingView);
    }

    protected void setupErrorView() {
        errorView = new ErrorView(getContext());

        errorView.setLayoutParams(new LinearLayout.LayoutParams(
                width,
                ViewGroup.LayoutParams.WRAP_CONTENT
        ));
        errorView.configure(new ErrorViewModel("Something went wrong. Please try again."));
        errorView.setOkButtonOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                OnButtonClicked(v, true);
            }
        });

        parentLayout.addView(errorView);
    }

    protected void setupContentView() {
        contentView = new LinearLayout(getContext());

        contentView.setOrientation(LinearLayout.VERTICAL);
        contentView.setLayoutParams(new LinearLayout.LayoutParams(
                width,
                ViewGroup.LayoutParams.WRAP_CONTENT
        ));
        contentView.setPadding(
                Utils.dpToPx(30),
                Utils.dpToPx(20),
                Utils.dpToPx(30),
                Utils.dpToPx(20)
        );

        parentLayout.addView(contentView);
    }

    public void configureWithModel(@NonNull DialogModel model) {
        this.interactionInterface = model.interactionInterface;
    }

    protected void OnButtonClicked(View v, boolean shouldDismissDialog) {
        if (shouldDismissDialog) {
            dismiss();
        }
    }

    public void show(DialogSubviewType type) {
        int loadingViewVisibility = type == DialogSubviewType.LOADING ? View.VISIBLE : View.GONE;
        int errorViewVisibility = type == DialogSubviewType.ERROR ? View.VISIBLE : View.GONE;
        int contentViewVisibility = type == DialogSubviewType.CONTENT ? View.VISIBLE : View.GONE;

        loadingView.setVisibility(loadingViewVisibility);
        errorView.setVisibility(errorViewVisibility);
        contentView.setVisibility(contentViewVisibility);

        this.show();
    }

    public static BaseDialog newInstance(Context ctx) {
        if (instance == null) {
            instance = new BaseDialog(ctx);
        }

        return instance;
    }
}