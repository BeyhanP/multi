package com.rollic.elephantsdk.Payload;

import android.util.Log;

import com.rollic.elephantsdk.Hyperlink.Hyperlink;
import com.rollic.elephantsdk.Utils.Constants;

import org.json.JSONException;
import org.json.JSONObject;

public class PersonalizedAdsPayload extends CustomPayload {
    public String privacy_policy_text;
    public String privacy_policy_url;
    public String decline_text_action_button;
    public String agree_text_action_button;

    public PersonalizedAdsPayload(JSONObject object) {
        super(object);

        try {
            this.privacy_policy_text = object.getString("privacy_policy_text");
            this.privacy_policy_url = object.getString("privacy_policy_url");
            this.decline_text_action_button = object.getString("decline_text_action_button");
            this.agree_text_action_button = object.getString("agree_text_action_button");
        } catch (JSONException e) {
            Log.d("JSONError", this.toString() + "  " + e.toString());
        }
    }

    @Override
    public Hyperlink[] getHyperlinks() {
        return new Hyperlink[] {
                new Hyperlink(Constants.PRIVACY_POLICY_MASK, privacy_policy_text, privacy_policy_url)
        };
    }
}
