package com.rollic.elephantsdk.Models.DialogModels;

import com.rollic.elephantsdk.Hyperlink.Hyperlink;
import com.rollic.elephantsdk.Interaction.InteractionInterface;
import com.rollic.elephantsdk.Payload.CustomPayload;

public class GenericDialogModel extends BaseDialogModel {
    public String title;
    public String content;
    public String actionButtonTitle;
    public Hyperlink[] hyperlinks;

    public GenericDialogModel(InteractionInterface interactionInterface, String content, String actionButtonTitle) {
        super(interactionInterface);

        this.title = "";
        this.content = content;
        this.actionButtonTitle = actionButtonTitle;
        this.hyperlinks = new Hyperlink[0];
    }

    public GenericDialogModel(InteractionInterface interactionInterface, String title,
                              String content, String actionButtonTitle, Hyperlink[] hyperlinks) {
        super(interactionInterface);

        this.title = title;
        this.content = content;
        this.actionButtonTitle = actionButtonTitle;
        this.hyperlinks = hyperlinks;
    }

    public GenericDialogModel(InteractionInterface interactionInterface, CustomPayload payload) {
        this(interactionInterface, payload.title , payload.content, payload.consent_text_action_button, payload.getHyperlinks());
    }
}
