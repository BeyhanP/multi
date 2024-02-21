package com.rollic.elephantsdk.Models.DialogModels;

import com.rollic.elephantsdk.Interaction.InteractionInterface;

public class BaseDialogModel {
    public InteractionInterface interactionInterface;

    BaseDialogModel(InteractionInterface interactionInterface) {
        this.interactionInterface = interactionInterface;
    }
}