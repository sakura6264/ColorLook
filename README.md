# **ColorLook颜色查看器**
查看颜色的小软件，可根据colors目录下的json文件中储存的颜色数据生成色卡，
支持两种简单的渐变色，支持导出png，jpeg，bmp格式的文件。  
使用C# WPF构建界面，调用C++ Dll函数生成色卡，使用FreeImage库生成和保存图片，
使用SDL2做预览。([FreeImage官网](https://freeimage.sourceforge.io),[SDL官网](https://www.libsdl.org))需要.NET FrameWork运行。  
编译后将编译得到的C#程序及dll文件放至Appdir目录下运行。
