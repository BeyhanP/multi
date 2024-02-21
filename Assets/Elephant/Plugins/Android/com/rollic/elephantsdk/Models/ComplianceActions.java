package com.rollic.elephantsdk.Models;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

public class ComplianceActions {
    public ComplianceAction[] actions;

    public ComplianceActions(JSONObject object) {
        try {
            JSONArray jsonArray = object.getJSONArray("actions");

            this.actions = new ComplianceAction[jsonArray.length()];

            for(int i = 0; i < jsonArray.length(); i++) {
                this.actions[i] = new ComplianceAction(jsonArray.getJSONObject(i));
            }
        } catch (JSONException e) {
            this.actions = new ComplianceAction[0];
        }
    }
}
