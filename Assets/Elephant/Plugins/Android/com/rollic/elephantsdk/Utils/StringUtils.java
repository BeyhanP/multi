package com.rollic.elephantsdk.Utils;

import android.text.Html;
import android.text.Spanned;

import com.rollic.elephantsdk.Hyperlink.Hyperlink;

public class StringUtils {
    public static String createHtmlUrlReference(String text, String url) {
        return "<a href='" + url + "'>" + text + "</a>";
    }

    public static Spanned configurePopUpHtmlContent(String content, Hyperlink[] hyperlinks) {
        content = content.replace("\n", "<br>");

        for(Hyperlink hyperlink: hyperlinks) {
            content = content.replace(hyperlink.mask, hyperlink.urlReference);
        }
        
        content = "<p style=\"text-align:center\">" + content + "</p>";

        return Html.fromHtml(content);
    }
}
