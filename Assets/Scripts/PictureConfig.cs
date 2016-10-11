using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using System.Drawing.Imaging;
[CreateAssetMenu(fileName = "TextureConfig.asset", menuName = "生成/图片配制")]
public class PictureConfig : ScriptableObject {
    //设置清晰度
    public float definition = 1000;
    //图片背景透明度
    public float transparent = 255;
}

