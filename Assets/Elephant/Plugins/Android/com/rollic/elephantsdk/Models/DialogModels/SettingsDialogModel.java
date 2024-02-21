package com.rollic.elephantsdk.Models.DialogModels;

import com.rollic.elephantsdk.Interaction.InteractionInterface;
import com.rollic.elephantsdk.Models.ComplianceAction;

public class SettingsDialogModel extends BaseDialogModel {

    public ComplianceAction[] complianceActions;

    public SettingsDialogModel(InteractionInterface interactionInterface, ComplianceAction[] complianceActions) {
        super(interactionInterface);

        this.complianceActions = complianceActions;
    }
}
