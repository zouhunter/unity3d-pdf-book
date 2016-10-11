using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using O2S.Components.PDFRender4NET;

public interface IPdfController {
    int Page { get; }
    bool GetFilePath(int page, UnityAction<byte[]> fileAction);
}
