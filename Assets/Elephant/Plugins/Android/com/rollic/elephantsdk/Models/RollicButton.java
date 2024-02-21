package com.rollic.elephantsdk.Models;

import android.content.Context;
import android.widget.Button;

public class RollicButton extends Button {

    public ComplianceAction complianceAction;

    public RollicButton(Context ctx) {
        super(ctx);
        this.setMaxLines(2);
    }

    public RollicButton(Context ctx, String title) {
        super(ctx);

        this.setText(title);
    }
    public RollicButton(Context ctx, ComplianceAction complianceAction) {
        super(ctx);

        this.setText(complianceAction.title);
        this.complianceAction = complianceAction;
    }
}
