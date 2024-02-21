package com.rollicads;

import android.content.Context;
import android.util.DisplayMetrics;

public class RollicAdsController {

    private Context ctx;

    private RollicAdsController(Context ctx) {
        this.ctx = ctx;
    }
    
    public static RollicAdsController create(Context ctx) {
        return new RollicAdsController(ctx);
    }

    public float convertDpToPixel(float dp) {
        return dp * ((float) ctx.getResources().getDisplayMetrics().densityDpi / DisplayMetrics.DENSITY_DEFAULT);
    }
}
