package com.rollic.elephantsdk.Payload;

import android.util.Log;

import org.json.JSONException;
import org.json.JSONObject;

public class URLPayload extends BasePayload {
    public String url;

    public URLPayload(JSONObject object) {
        super(object);

        try {
            this.url = object.getString("url");
        } catch (JSONException e) {
            Log.d("JSONError", this.toString() + "  " + e.toString());
        }
    }
}
