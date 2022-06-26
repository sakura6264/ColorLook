// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include<FreeImage.h>
#include<SDL.h>
#include<vector>
#include<math.h>

BOOL APIENTRY DllMain(
    HANDLE hModule,	   // Handle to DLL module 
    DWORD ul_reason_for_call,
    LPVOID lpReserved)     // Reserved
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
        break;
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}
bool GeneralShow(TCHAR* title,int width,int height,FIBITMAP* mshow)
{
    SDL_Init(SDL_INIT_VIDEO);
    char titleUTF8[64] = { 0 };
    WideCharToMultiByte(CP_UTF8, 0, title, -1, titleUTF8, 64, NULL, NULL);
    SDL_Window* show = SDL_CreateWindow(titleUTF8, 100, 100, width, height, SDL_WINDOW_SHOWN);
    if (show == NULL)return false;
    SDL_Renderer* ren = SDL_CreateRenderer(show, -1, SDL_RENDERER_ACCELERATED | SDL_RENDERER_PRESENTVSYNC);
    SDL_Surface* surface = SDL_CreateRGBSurfaceFrom(FreeImage_GetBits(mshow), width, height, 24, FreeImage_GetPitch(mshow), 0, 0, 0, 0);
    SDL_Texture* tex = SDL_CreateTextureFromSurface(ren, surface);
    SDL_FreeSurface(surface);
    SDL_Event evt;
    SDL_Rect rect{ 0,0,width,height };
    for (;;)
    {
        SDL_PollEvent(&evt);
        if (evt.type == SDL_QUIT)break;
        SDL_RenderClear(ren);
        SDL_RenderCopy(ren, tex, &rect, &rect);
        SDL_RenderPresent(ren);
    }
    SDL_DestroyRenderer(ren);
    SDL_DestroyTexture(tex);
    SDL_DestroyWindow(show);
    SDL_Quit();
    return true;
}
bool GeneralSave(FIBITMAP* mshow,TCHAR * target,int opt=-1)
{
    //save the image.
    bool ret=false;
    bool known = false;
    FREE_IMAGE_FORMAT FIF = FreeImage_GetFIFFromFilenameU(target);
    if (FIF==FIF_JPEG)
    {
        known = true;
        if (opt < 0 || opt>100)return false;
    }
    else if (FIF == FIF_PNG)
    {
        known = true;
        if (opt < 0 || opt>9)return false;
    }
    if (opt >=0&&known)
        ret = FreeImage_SaveU(FIF, mshow, target, opt);
    else ret = FreeImage_SaveU(FIF, mshow, target);
    return ret;
}
//generate Mat.
bool MonochromeGenerate(FIBITMAP* mshow, int R, int G, int B)
{
    if (R < 0 || G < 0 || B < 0 || R>0xff || G>0xff || B>0xff)return false;
    RGBQUAD RGBbuffer = {0};
    for (unsigned int x = 0; x < FreeImage_GetWidth(mshow); x++)
    {
        for (unsigned int y = 0; y < FreeImage_GetHeight(mshow); y++)
        {
            RGBbuffer.rgbRed = R;
            RGBbuffer.rgbGreen = G;
            RGBbuffer.rgbBlue = B;
            FreeImage_SetPixelColor(mshow, x, y, &RGBbuffer);
        }
    }
    return true;
}
bool GradientGenerate(FIBITMAP* mshow, int R0, int G0, int B0, int R1, int G1, int B1,double ang)
{
    if (R0 < 0 || G0 < 0 || B0 < 0 || R0>0xff || G0>0xff || B0>0xff || R1 < 0 || G1 < 0 || B1 < 0 || R1>0xff || G1>0xff || B1>0xff)return false;
    RGBQUAD RGBbuffer = { 0 };
    for (unsigned int x = 0; x < FreeImage_GetWidth(mshow); x++)
    {
        for (unsigned int y = 0; y < FreeImage_GetHeight(mshow); y++)
        {
            double c1 = y * tan(ang) + x;
            double c2 = FreeImage_GetHeight(mshow) * tan(ang) + FreeImage_GetWidth(mshow);
            RGBbuffer.rgbRed = int((R1 * c1 + R0 * (c2 - c1)) / c2);
            RGBbuffer.rgbGreen = int((G1 * c1 + G0 * (c2 - c1)) / c2);
            RGBbuffer.rgbBlue =  int((B1 * c1 + B0 * (c2 - c1)) / c2);
            FreeImage_SetPixelColor(mshow, x, y, &RGBbuffer);
        }
    }
    return true;
}
bool CenterGenerate(FIBITMAP* mshow, int R0, int G0, int B0, int R1, int G1, int B1)
{
    if (R0 < 0 || G0 < 0 || B0 < 0 || R0>0xff || G0>0xff || B0>0xff || R1 < 0 || G1 < 0 || B1 < 0 || R1>0xff || G1>0xff || B1>0xff)return false;
    RGBQUAD RGBbuffer = { 0 };
    for (unsigned int x = 0; x < FreeImage_GetWidth(mshow); x++)
    {
        for (unsigned int y = 0; y < FreeImage_GetHeight(mshow); y++)
        {
            int w = FreeImage_GetWidth(mshow);
            int h = FreeImage_GetHeight(mshow);
            int c1 = (x + x - w) * (x + x - w) + (y + y - h) * (y + y - h);
            int c2 = w * w + h * h;
            RGBbuffer.rgbRed = int((R1 * c1 + R0 * (c2 - c1)) / c2);
            RGBbuffer.rgbGreen = int((G1 * c1 + G0 * (c2 - c1)) / c2);
            RGBbuffer.rgbBlue = int((B1 * c1 + B0 * (c2 - c1)) / c2);
            FreeImage_SetPixelColor(mshow, x, y, &RGBbuffer);
        }
    }
    return true;
}
//export
extern "C" __declspec(dllexport) bool IMGMonochromeShow(int width, int height, int R, int G, int B,TCHAR* title)
{
    FreeImage_Initialise(TRUE);
    FIBITMAP* mshow = FreeImage_Allocate(width, height, 24);
    bool ret = MonochromeGenerate(mshow, R, G, B);
    if (!ret)return false;
    ret = GeneralShow(title, width, height, mshow);
    if (!ret)return false;
    FreeImage_Unload(mshow);
    FreeImage_DeInitialise();
    return true;
}
extern "C" __declspec(dllexport) bool IMGMonochromeSave(int width, int height, int R, int G, int B,TCHAR* target,int opt=-1)
{
    FreeImage_Initialise(TRUE);
    FIBITMAP* mshow = FreeImage_Allocate(width, height, 24);
    bool ret = MonochromeGenerate(mshow, R, G, B);
    if (!ret)return false;
    ret = GeneralSave(mshow, target, opt);
    FreeImage_Unload(mshow);
    FreeImage_DeInitialise();
    return ret;
}
extern "C" __declspec(dllexport) bool IMGGradientShow(int width, int height, int R0, int G0, int B0, int R1, int G1, int B1, double ang, TCHAR* title)
{
    FreeImage_Initialise(TRUE);
    FIBITMAP* mshow = FreeImage_Allocate(width, height, 24);
    bool ret = GradientGenerate(mshow, R0, G0, B0, R1, G1, B1, ang);
    if (!ret)return false;
    ret = GeneralShow(title, width, height, mshow);
    if (!ret)return false;
    FreeImage_Unload(mshow);
    FreeImage_DeInitialise();
    return true;
}
extern "C" __declspec(dllexport) bool IMGGradientSave(int width, int height, int R0, int G0, int B0, int R1, int G1, int B1, double ang, TCHAR* target,int opt=-1)
{
    FreeImage_Initialise(TRUE);
    FIBITMAP* mshow = FreeImage_Allocate(width, height, 24);
    bool ret = GradientGenerate(mshow, R0, G0, B0, R1, G1, B1, ang);
    if (!ret)return false;
    ret = GeneralSave(mshow, target, opt);
    FreeImage_Unload(mshow);
    FreeImage_DeInitialise();
    return ret;
}
extern "C" __declspec(dllexport) bool IMGCenterShow(int width, int height, int R0, int G0, int B0, int R1, int G1, int B1, TCHAR* title)
{
    FreeImage_Initialise(TRUE);
    FIBITMAP* mshow = FreeImage_Allocate(width, height, 24);
    bool ret = CenterGenerate(mshow, R0, G0, B0, R1, G1, B1);
    if (!ret)return false;
    ret = GeneralShow(title, width, height, mshow);
    if (!ret)return false;
    FreeImage_Unload(mshow);
    FreeImage_DeInitialise();
    return true;
}
extern "C" __declspec(dllexport) bool IMGCenterSave(int width, int height, int R0, int G0, int B0, int R1, int G1, int B1, TCHAR* target, int opt = -1)
{
    FreeImage_Initialise(TRUE);
    FIBITMAP* mshow = FreeImage_Allocate(width, height, 24);
    bool ret = CenterGenerate(mshow, R0, G0, B0, R1, G1, B1);
    if (!ret)return false;
    ret = GeneralSave(mshow, target, opt);
    FreeImage_Unload(mshow);
    FreeImage_DeInitialise();
    return ret;
}
