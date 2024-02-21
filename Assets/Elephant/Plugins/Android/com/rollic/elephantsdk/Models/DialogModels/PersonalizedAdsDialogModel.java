package com.rollic.elephantsdk.Models.DialogModels;

import com.rollic.elephantsdk.Hyperlink.Hyperlink;
import com.rollic.elephantsdk.Interaction.InteractionInterface;
import com.rollic.elephantsdk.Models.ActionType;
import com.rollic.elephantsdk.Models.ComplianceAction;
import com.rollic.elephantsdk.Payload.PersonalizedAdsPayload;

public class PersonalizedAdsDialogModel extends GenericDialogModel {

    public ActionType action;
    public String declineButtonTitle;
    public String agreeButtonTitle;


    public PersonalizedAdsDialogModel(InteractionInterface interactionInterface, ActionType action, String title,
                               String content, String declineButtonTitle, String agreeButtonTitle,
                               String backToGameButtonTitle, Hyperlink[] hyperlinks) {
        super(interactionInterface, title, content, backToGameButtonTitle, hyperlinks);

        this.action = action;
        this.declineButtonTitle = declineButtonTitle;
        this.agreeButtonTitle = agreeButtonTitle;
    }

    public PersonalizedAdsDialogModel(InteractionInterface interactionInterface, ComplianceAction complianceAction) {
        super(interactionInterface, complianceAction.getPayload());

        PersonalizedAdsPayload payload = complianceAction.getPayload();

        this.action = complianceAction.action;
        this.declineButtonTitle = payload.decline_text_action_button;
        this.agreeButtonTitle = payload.agree_text_action_button;
    }
}
