using UnityEngine;
using System.Collections;
using System.Drawing.Imaging;

[System.Serializable]
public class PDF2TextureData
{
    //PDF文件下载路劲
    public string PdfPath;
    //pdf文件名
    public string pdfName;
    //PDF图片保存路径
    public string imageOutputPath;
    //生成的图片的文件夹名
    public string ImageFilesPath;
    //设置所需图片格式
    public ImageFormat imageFormat = ImageFormat.Png;
    //设置清晰度
    public float definition  = 10;
    //图片背景透明度
    public float transparent = 10;
}