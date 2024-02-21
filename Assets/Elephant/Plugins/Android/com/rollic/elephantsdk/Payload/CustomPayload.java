package com.rollic.elephantsdk.Payload;

import android.util.Log;

import com.rollic.elephantsdk.Hyperlink.Hyperlink;
import com.rollic.elephantsdk.Hyperlink.IHyperlinkManager;

import org.json.JSONException;
import org.json.JSONObject;

public class CustomPayload extends BasePayload implements IHyperlinkManager {
    public String title;
    public String content;
    public String consent_text_action_button;

    public CustomPayload(JSONObject object) {
        super(object);

        try {
            this.title = object.getString("title");
            this.content = object.getString("content");
            this.consent_text_action_button = object.getString("consent_text_action_button");
        } catch (JSONException e) {
            Log.d("JSONError", this.toString() + "  " + e.toString());
        }
    }

    @Override
    public Hyperlink[] getHyperlinks() {
        return new Hyperlink[0];
    }
}
