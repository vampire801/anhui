LOCAL_PATH:= $(call my-dir)

# bzip2
include $(CLEAR_VARS)
LOCAL_MODULE := bzip2
LOCAL_SRC_FILES := blocksort.c bzlib.c compress.c crctable.c decompress.c huffman.c randtable.c
include $(BUILD_STATIC_LIBRARY)

#bspatch
include $(CLEAR_VARS)
LOCAL_MODULE := bspatch
LOCAL_SRC_FILES:= com_ibluejoy_anhuishuangxi_wxapi_WXEntryActivity.c
LOCAL_STATIC_LIBRARIES := bzip2
LOCAL_LDLIBS := -llog
include $(BUILD_SHARED_LIBRARY)