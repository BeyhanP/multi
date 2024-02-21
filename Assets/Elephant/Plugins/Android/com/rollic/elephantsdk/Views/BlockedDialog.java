package com.rollic.elephantsdk.Views;

import android.content.Context;
import android.view.View;

import com.rollic.elephantsdk.Interaction.InteractionType;
import com.rollic.elephantsdk.Models.DialogModels.BlockedDialogModel;
import com.rollic.elephantsdk.Models.WarningViewModel;

public class BlockedDialog extends GenericDialog<BlockedDialogModel> {

    public static BlockedDialog instance;

    WarningView warningView;

    public BlockedDialog(Context ctx) {
        super(ctx);

        setupWarningView();
    }

    private void setupWarningView() {
        warningView = new WarningView(getContext());
        int childIndex = contentView.getChildCount()-1;

        contentView.addView(warningView, childIndex);
    }

    @Override
    public void configureWithModel(BlockedDialogModel model) {
        super.configureWithModel(model);

        configureWarningView(new WarningViewModel(model.warningContent));
    }

    private void configureWarningView(WarningViewModel model) {
        int visibility = model.content.isEmpty() ? View.GONE : View.VISIBLE;

        warningView.configure(model);
        warningView.setVisibility(visibility);
    }

    @Override
    protected void OnButtonClicked(View v, boolean shouldDismissDialog) {
        super.OnButtonClicked(v, shouldDismissDialog);

        interactionInterface.OnButtonClick(InteractionType.DELETE_REQUEST_CANCEL);
    }

    public static BlockedDialog newInstance(Context ctx) {
        if (instance == null) {
            instance = new BlockedDialog(ctx);
        }

        return instance;
    }
}
