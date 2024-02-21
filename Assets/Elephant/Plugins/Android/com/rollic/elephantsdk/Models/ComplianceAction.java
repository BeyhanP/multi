package com.rollic.elephantsdk.Models;

import android.util.Log;

import com.rollic.elephantsdk.Payload.BasePayload;
import com.rollic.elephantsdk.Payload.CustomPayload;
import com.rollic.elephantsdk.Payload.PersonalizedAdsPayload;
import com.rollic.elephantsdk.Payload.URLPayload;

import org.json.JSONException;
import org.json.JSONObject;

public class ComplianceAction {
    public String title;
    public ActionType action;
    public BasePayload payload;

    ComplianceAction(String title, ActionType action, BasePayload payload) {
        this.title = title;
        this.action = action;
        this.payload = payload;
    }

    ComplianceAction(String title, String actionTypeString, BasePayload payload) {
        this.title = title;
        this.action = ActionType.valueOf(actionTypeString);
        this.payload = payload;
    }

    public ComplianceAction(JSONObject object) {
        try {
            this.title = object.getString("title");
            this.action = ActionType.valueOf(object.getString("action"));
            String payloadStr = object.getString("payload");
            JSONObject payloadJson = new JSONObject(payloadStr);

            switch (this.action) {
                case URL:
                    this.payload = new URLPayload(payloadJson);
                    break;
                case CCPA: case GDPR_AD_CONSENT:
                    this.payload = new PersonalizedAdsPayload(payloadJson);
                    break;
                case CUSTOM_POPUP:
                    this.payload = new CustomPayload(payloadJson);
                    break;
                default: break;
            }
        } catch (JSONException e) {
            Log.d("JSONError", this.toString() + "  " + e.toString());
        }
    }

    public <T extends BasePayload> T getPayload() {
        return (T) payload;
    }
}