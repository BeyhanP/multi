package com.rollic.elephantsdk.Models.DialogModels;

import com.rollic.elephantsdk.Hyperlink.Hyperlink;
import com.rollic.elephantsdk.Interaction.InteractionInterface;

public class BlockedDialogModel extends GenericDialogModel {

    public String warningContent;

    public BlockedDialogModel(InteractionInterface interactionInterface, String title,
                              String content, String warningContent, String actionButtonTitle, Hyperlink[] hyperlinks) {
        super(interactionInterface, title, content, actionButtonTitle, hyperlinks);

        this.warningContent = warningContent;
    }
}
