package com.rollic.elephantsdk.Hyperlink;

import com.rollic.elephantsdk.Utils.StringUtils;

public class Hyperlink {
    public String mask;
    public String urlReference;

    public Hyperlink(String mask, String text, String url) {
        this.mask = mask;
        this.urlReference = StringUtils.createHtmlUrlReference(text, url);
    }
}
