package com.rollic.elephantsdk;

import android.Manifest;
import android.app.Activity;
import android.app.ActivityManager;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.graphics.Typeface;
import android.net.Uri;
import android.os.BatteryManager;
import android.os.Build;
import android.os.RemoteException;
import android.text.Html;
import android.text.SpannableString;
import android.text.Spanned;
import android.text.TextUtils;
import android.text.method.LinkMovementMethod;
import android.text.style.StyleSpan;
import android.util.Log;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.core.app.ActivityCompat;

import com.android.installreferrer.api.InstallReferrerClient;
import com.android.installreferrer.api.InstallReferrerStateListener;
import com.android.installreferrer.api.ReferrerDetails;
import com.android.volley.AuthFailureError;
import com.android.volley.NoConnectionError;
import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;
import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.Task;
import com.rollic.elephantsdk.Models.ActionType;
import com.rollic.elephantsdk.Models.DialogModels.BlockedDialogModel;
import com.rollic.elephantsdk.Models.DialogModels.GenericDialogModel;
import com.rollic.elephantsdk.Models.DialogModels.PersonalizedAdsDialogModel;
import com.rollic.elephantsdk.Models.DialogModels.SettingsDialogModel;
import com.rollic.elephantsdk.Models.DialogSubviewType;
import com.google.android.gms.ads.identifier.AdvertisingIdClient;
import com.google.android.gms.common.GooglePlayServicesNotAvailableException;
import com.google.android.gms.common.GooglePlayServicesRepairableException;
import com.rollic.elephantsdk.Hyperlink.Hyperlink;
import com.rollic.elephantsdk.Interaction.InteractionInterface;
import com.rollic.elephantsdk.Interaction.InteractionType;
import com.rollic.elephantsdk.Models.ComplianceActions;
import com.rollic.elephantsdk.Utils.Constants;
import com.rollic.elephantsdk.Views.BlockedDialog;
import com.rollic.elephantsdk.Views.GenericDialog;
import com.rollic.elephantsdk.Views.PersonalizedAdsConsentView;
import com.rollic.elephantsdk.Views.SettingsView;
import com.unity3d.ads.metadata.MetaData;
import com.unity3d.player.R;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.lang.reflect.Field;
import java.nio.charset.StandardCharsets;
import java.util.Arrays;
import java.util.List;
import java.util.Locale;
import java.util.Map;
import java.util.HashMap;


public class ElephantController implements InteractionInterface {

    private static final String LOG_TAG = "[ELEPHANT SDK]";
    private RequestQueue queue;

    private Context ctx;


    private ElephantController(Context ctx) {
        this.ctx = ctx;
        this.queue = Volley.newRequestQueue(this.ctx);
    }


    public static ElephantController create(Context ctx) {
        return new ElephantController(ctx);
    }

    public void ElephantPost(final String url, final String body, final String gameID, final String authToken, int _tryCount) {

        try {
            final int tryCount = _tryCount + 1;
            StringRequest stringRequest = new StringRequest(Request.Method.POST, url,
                    new Response.Listener<String>() {
                        @Override
                        public void onResponse(String response) {
                            // Display the first 500 characters of the response string.
                            Log.e(LOG_TAG, "onResponse: " + response);
                        }
                    }, new Response.ErrorListener() {
                @Override
                public void onErrorResponse(VolleyError error) {

                    try {
                        boolean isOffline = false;
                        int statusCode = -1;

                        if (error instanceof NoConnectionError) {
                            isOffline = true;
                        }

                        if (error.networkResponse != null) {
                            statusCode = error.networkResponse.statusCode;
                        }

                        JSONObject failedReq = new JSONObject();
                        failedReq.accumulate("url", url);
                        failedReq.accumulate("isOffline", isOffline);
                        failedReq.accumulate("statusCode", statusCode);
                        failedReq.accumulate("data", body);
                        failedReq.accumulate("tryCount", tryCount);
                        com.unity3d.player.UnityPlayer.UnitySendMessage("Elephant", "FailedRequest", failedReq.toString());
                    } catch (JSONException e) {
                        e.printStackTrace();
                    }

                    Log.e(LOG_TAG, "error: " + error.networkResponse);
                }
            }) {

                @Override
                public Map<String, String> getHeaders() throws AuthFailureError {
                    Map<String, String> headers = new HashMap<>();
                    headers.put("Content-Type", "application/json; charset=utf-8");
                    headers.put("Authorization", authToken);
                    headers.put("GameID", gameID);
                    return headers;
                }

                @Override
                public byte[] getBody() throws AuthFailureError {
                    return body.getBytes(StandardCharsets.UTF_8);
                }
            };


            queue.add(stringRequest);

        } catch (Exception e) {
            e.printStackTrace();
        }

    }

    public void showAlertDialog(String title, String message) {
        if (message.contains("{{tos}}")) {
            message = message.replace("{{tos}}", "<a href=\"" + title + "\">Terms of Service</a>");

            AlertDialog alertDialog = new AlertDialog.Builder(ctx)
                    .setTitle(title)
                    .setMessage(Html.fromHtml(message))
                    .setCancelable(true)
                    .setPositiveButton(android.R.string.yes, new DialogInterface.OnClickListener() {
                        public void onClick(DialogInterface dialog, int which) {
                            dialog.dismiss();
                        }
                    }).show();

            ((TextView) alertDialog.findViewById(android.R.id.message)).setMovementMethod(LinkMovementMethod.getInstance());
        } else {
            new AlertDialog.Builder(ctx)
                    .setTitle(title)
                    .setMessage(message)
                    .setCancelable(true)
                    .setPositiveButton(android.R.string.yes, new DialogInterface.OnClickListener() {
                        public void onClick(DialogInterface dialog, int which) {
                            dialog.dismiss();
                        }
                    }).show();
        }
    }

    public void showForceUpdate(String title, String message) {
        new AlertDialog.Builder(ctx)
                .setTitle(title)
                .setMessage(message)
                .setCancelable(false)
                .setPositiveButton(android.R.string.yes, new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int which) {
                        final String appPackageName = ctx.getPackageName();
                        try {
                            ctx.startActivity(new Intent(Intent.ACTION_VIEW, Uri.parse("market://details?id=" + appPackageName)));
                        } catch (android.content.ActivityNotFoundException anfe) {
                            ctx.startActivity(new Intent(Intent.ACTION_VIEW, Uri.parse("https://play.google.com/store/apps/details?id=" + appPackageName)));
                        }

                    }
                }).show();

    }

    private void showConsent(String subviewType, String content, String buttonTitle,
                             String privacyPolicyText, String privacyPolicyUrl,
                             String tosText, String TosUrl,
                             String dataRequestText, String dataRequestUrl) {
        Hyperlink[] hyperlinks = {
                new Hyperlink(Constants.PRIVACY_POLICY_MASK, privacyPolicyText, privacyPolicyUrl),
                new Hyperlink(Constants.TERMS_OF_SERVICE_MASK, tosText, TosUrl),
                new Hyperlink(Constants.PERSONAL_DATA_REQUEST_MASK, dataRequestText, dataRequestUrl)
        };
        GenericDialogModel model = new GenericDialogModel(this, "", content, buttonTitle, hyperlinks);
        GenericDialog dialog = GenericDialog.newInstance(ctx);

        dialog.configureWithModel(model);

        dialog.configureButtonActionHandler(new GenericDialog.ButtonActionHandler() {
            @Override
            public void onButtonClickHandler() {
                if (dataRequestText.isEmpty()) {
                    OnButtonClick(InteractionType.TOS_ACCEPT);
                }
            }
        });

        dialog.show(DialogSubviewType.valueOf(subviewType));
    }

    public void showCcpaDialog(String action, String title, String content,
                               String privacyPolicyText, String privacyPolicyUrl,
                               String declineActionButtonText, String agreeActionButtonText,
                               String backToGameActionButtonText) {
        ActionType actionEnum = ActionType.valueOf(action);
        Hyperlink hyperlinks[] = {new Hyperlink(Constants.PRIVACY_POLICY_MASK, privacyPolicyText, privacyPolicyUrl)};
        PersonalizedAdsConsentView personalizedAdsConsentView =
                PersonalizedAdsConsentView.newInstance(ctx);
        PersonalizedAdsDialogModel model =
                new PersonalizedAdsDialogModel(this, actionEnum, title, content,
                        declineActionButtonText, agreeActionButtonText, backToGameActionButtonText, hyperlinks);
        personalizedAdsConsentView.configureWithModel(model);

        personalizedAdsConsentView.show(DialogSubviewType.CONTENT);
    }

    public void showSettingsView(String subviewType, String actions) {
        SettingsView settingsView = SettingsView.newInstance(ctx);

        try {
            JSONObject jsonObject = new JSONObject(actions);
            ComplianceActions complianceActions = new ComplianceActions(jsonObject);
            SettingsDialogModel model = new SettingsDialogModel(this, complianceActions.actions);

            settingsView.configureWithModel(model);
        } catch (JSONException e) {
            e.printStackTrace();
        }

        settingsView.show(DialogSubviewType.valueOf(subviewType));
    }

    public void showBlockedDialog(String title, String content, String warningContent, String buttonTitle) {
        BlockedDialog blockedDialog = BlockedDialog.newInstance(ctx);
        BlockedDialogModel model = new BlockedDialogModel(this,
                title, content, warningContent, buttonTitle, new Hyperlink[]{});

        blockedDialog.configureWithModel(model);

        blockedDialog.show(DialogSubviewType.CONTENT);
    }

    public void showNetworkOfflineDialog(String content, String buttonTitle) {
        GenericDialog dialog = GenericDialog.newInstance(ctx);
        GenericDialogModel model = new GenericDialogModel(this, content, buttonTitle);

        dialog.configureWithModel(model);

        dialog.configureButtonActionHandler(new GenericDialog.ButtonActionHandler() {
            @Override
            public void onButtonClickHandler() {
                OnButtonClick(InteractionType.RETRY_CONNECTION);
            }
        });

        dialog.show(DialogSubviewType.CONTENT);
    }

    public String getBuildNumber() {
        if (getBuildConfigValue() == null) {
            return "";
        }

        return getBuildConfigValue() + "";
    }

    private Object getBuildConfigValue() {
        try {
            Class<?> clazz = Class.forName(ctx.getPackageName() + ".BuildConfig");
            Field field = clazz.getField("VERSION_CODE");
            return field.get(null);
        } catch (ClassNotFoundException e) {
            e.printStackTrace();
        } catch (NoSuchFieldException e) {
            e.printStackTrace();
        } catch (IllegalAccessException e) {
            e.printStackTrace();
        }
        return null;
    }

    public String getLocale() {
        String locale;
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.LOLLIPOP) {
            locale = Locale.getDefault().toLanguageTag();
        } else {
            locale = Locale.getDefault().toString();
        }

        return locale;
    }

    public String FetchAdId() {
        String adId = "";

        try {
            AdvertisingIdClient.Info adIdInfo = AdvertisingIdClient.getAdvertisingIdInfo(ctx);
            if (adIdInfo == null) {
                return adId;
            }

            adId = adIdInfo.getId() != null ? adIdInfo.getId() : "";

        } catch (IOException e) {
            e.printStackTrace();
        } catch (GooglePlayServicesNotAvailableException e) {
            e.printStackTrace();
        } catch (GooglePlayServicesRepairableException e) {
            e.printStackTrace();
        }

        return adId;
    }

    public int gameMemoryUsage() {
        try {
            ActivityManager mgr = (ActivityManager) ctx.getSystemService(Context.ACTIVITY_SERVICE);
            List<ActivityManager.RunningAppProcessInfo> processes = mgr.getRunningAppProcesses();
            double memoryUsage = 0;

            if (processes.size() == 0) return 0;

            for (ActivityManager.RunningAppProcessInfo p : processes) {
                int[] pids = new int[1];
                pids[0] = p.pid;
                android.os.Debug.MemoryInfo[] MI = mgr.getProcessMemoryInfo(pids);
                if (MI[0] == null || MI[0].getTotalPss() <= 0) continue;
                memoryUsage = MI[0].getTotalPss() / 1000.0;
            }
            if (memoryUsage <= 0) return 0;
            return (int) memoryUsage;
        } catch (Exception e) {
            e.printStackTrace();
        }
        return -1;
    }

    public int gameMemoryUsagePercentage() {
        try {
            double memoryUsage = gameMemoryUsage();
            if (memoryUsage <= 0) return 0;

            ActivityManager.MemoryInfo mi = new ActivityManager.MemoryInfo();
            ActivityManager activityManager = (ActivityManager) ctx.getSystemService(Context.ACTIVITY_SERVICE);
            activityManager.getMemoryInfo(mi);
            if (mi.totalMem <= 0) return 0;
            double totalMemory = (double) mi.totalMem / 0x100000L;
            return ((int) memoryUsage * 100) / (int) totalMemory;
        } catch (Exception e) {
            e.printStackTrace();
        }
        return -1;
    }

    public float getBatteryLevel() {
        IntentFilter intentFilter = new IntentFilter(Intent.ACTION_BATTERY_CHANGED);
        Intent batteryStatus = ctx.registerReceiver(null, intentFilter);

        int level = batteryStatus.getIntExtra(BatteryManager.EXTRA_LEVEL, -1);
        int scale = batteryStatus.getIntExtra(BatteryManager.EXTRA_SCALE, -1);

        return level / (float) scale;
    }

    public long getFirstInstallTime() {
        try {
            return ctx.getPackageManager().getPackageInfo(ctx.getPackageName(), 0).firstInstallTime;
        } catch (PackageManager.NameNotFoundException e) {
            return 0;
        }
    }

    public String test() {
        Log.e(LOG_TAG, "test called");

        return "Hello from Elephant android plugin ";
    }

    @Override
    public void OnButtonClick(InteractionType interactionType) {
        // TO DO: Handle popup button interactions with InteractionType.

        switch (interactionType) {
            case TOS_ACCEPT:
                com.unity3d.player.UnityPlayer.UnitySendMessage("Elephant", "UserConsentAction", "TOS_ACCEPT");
                break;
            case GDPR_AD_CONSENT_AGREE:
                com.unity3d.player.UnityPlayer.UnitySendMessage("Elephant", "UserConsentAction", "GDPR_AD_CONSENT_AGREE");
                break;
            case GDPR_AD_CONSENT_DECLINE:
                com.unity3d.player.UnityPlayer.UnitySendMessage("Elephant", "UserConsentAction", "GDPR_AD_CONSENT_DECLINE");
                break;
            case PERSONALIZED_ADS_AGREE:
                com.unity3d.player.UnityPlayer.UnitySendMessage("Elephant", "UserConsentAction", "PERSONALIZED_ADS_AGREE");
                break;
            case PERSONALIZED_ADS_DECLINE:
                com.unity3d.player.UnityPlayer.UnitySendMessage("Elephant", "UserConsentAction", "PERSONALIZED_ADS_DECLINE");
                break;
            case CALL_DATA_REQUEST:
                com.unity3d.player.UnityPlayer.UnitySendMessage("Elephant", "UserConsentAction", "CALL_DATA_REQUEST");
                break;
            case DELETE_REQUEST_CANCEL:
                com.unity3d.player.UnityPlayer.UnitySendMessage("Elephant", "UserConsentAction", "DELETE_REQUEST_CANCEL");
                break;
            case RETRY_CONNECTION:
                com.unity3d.player.UnityPlayer.UnitySendMessage("Elephant", "UserConsentAction", "RETRY_CONNECTION");
        }
    }

}