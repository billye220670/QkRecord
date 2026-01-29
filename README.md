# QkRecord - 轻量级音频录制工具

<div align="center">

![Version](https://img.shields.io/badge/version-1.0.1-blue.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)
![.NET](https://img.shields.io/badge/.NET-6.0-512BD4.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)

一款简洁、高效的 Windows 音频录制工具，支持系统音频环回录制

[下载最新版本](https://github.com/billye220670/QkRecord/releases/latest) | [问题反馈](https://github.com/billye220670/QkRecord/issues)

</div>

---

## ✨ 功能特性

- 🎙️ **高质量录制** - 44.1kHz 采样率，CD 级音质
- 🔊 **系统音频捕获** - 录制电脑正在播放的任何声音（音乐、视频、游戏等）
- 💾 **多格式支持** - 支持 WAV（无损）和 MP3（128kbps）格式
- ⏱️ **实时计时** - 录制过程中显示实时时长
- 📁 **智能命名** - 自动时间戳命名，防止文件覆盖
- 🎨 **现代 UI** - 简洁直观的界面设计
- 📦 **开箱即用** - 单文件发布，无需安装 .NET 运行时
- 🪟 **置顶显示** - 窗口始终保持在最前方，方便操作

## 📸 界面预览

```
┌─────────────────────────────────┐
│  QkRecord v1.0.0           ✕   │
├─────────────────────────────────┤
│                                 │
│          ┌─────────┐            │
│          │    🎤   │            │
│          │         │            │  ← 点击开始录制
│          └─────────┘            │
│                                 │
│  C:\...\recording_20250129...   │  ← 点击选择保存路径
│                                 │
└─────────────────────────────────┘
```

**录制状态：**
- 🔵 **蓝色圆形按钮** = 就绪状态，点击开始录制
- 🔴 **红色圆形按钮 + 计时器** = 正在录制，点击停止

## 🚀 快速开始

### 下载安装

1. 访问 [Releases 页面](https://github.com/billye220670/QkRecord/releases/latest)
2. 下载 `QkRecord-v1.0.1-win-x64.zip`
3. 解压到任意目录
4. 双击运行 `QkRecord.exe`

### 使用方法

#### 快速录制
1. 启动 `QkRecord.exe`
2. 点击蓝色圆形按钮开始录制
3. 播放你想录制的音频（音乐、视频等）
4. 点击红色按钮停止录制
5. 录音文件自动保存在 `Recordings` 文件夹

#### 自定义保存路径
1. 点击底部的文件路径链接
2. 在对话框中选择保存位置和文件名
3. 选择格式（WAV 或 MP3）
4. 点击录制按钮

### 系统要求

- **操作系统**: Windows 10/11 (64位)
- **磁盘空间**:
  - 应用程序：~150 MB
  - 录音文件：WAV 约 10 MB/分钟，MP3 约 1 MB/分钟
- **其他**: 需要有音频输出设备（扬声器/耳机）

## ⚠️ Windows SmartScreen 警告说明

首次运行时，Windows 可能会显示 SmartScreen 警告：

```
Windows 已保护你的电脑
Microsoft Defender SmartScreen 阻止了无法识别的应用启动
```

**这是正常现象**，原因是应用程序未经过代码签名认证。

**解决方法：**
1. 点击 **"更多信息"**
2. 点击 **"仍要运行"**

**为什么会出现这个警告？**
- 本应用是开源软件，未购买昂贵的代码签名证书（$100-500/年）
- 您可以查看[源代码](https://github.com/billye220670/QkRecord)确认安全性
- 也可以自行下载源码编译

## 🛠️ 技术栈

- **框架**: .NET 6.0 + WPF
- **音频处理**:
  - [NAudio](https://github.com/naudio/NAudio) - 音频捕获
  - [NAudio.Lame](https://github.com/Corey-M/NAudio.Lame) - MP3 编码
- **音频接口**: WASAPI Loopback (系统音频环回)

## 🔨 开发者指南

### 从源码编译

#### 前置要求
- .NET 6.0 SDK
- Windows 10/11
- Visual Studio 2022 / Rider / VS Code

#### 编译步骤

```bash
# 克隆仓库
git clone https://github.com/billye220670/QkRecord.git
cd QkRecord

# 恢复依赖
dotnet restore

# 开发编译
dotnet build

# 运行
dotnet run

# 发布单文件版本
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
```

### 项目结构

```
QkRecord/
├── App.xaml                          # 应用程序定义
├── App.xaml.cs                       # 应用程序入口
├── MainWindow.xaml                   # 主窗口 UI
├── MainWindow.xaml.cs                # 主窗口逻辑
├── CoreAudio/
│   └── NaudioLoopbackRecorder.cs    # 音频录制引擎
├── AudioRecorder.csproj              # 项目配置
├── app.manifest                      # Windows 清单
├── favicon.ico                       # 应用图标
├── CLAUDE.md                         # 项目技术文档
└── README.md                         # 本文件
```

### 核心架构

```
┌─────────────────────────────────────────┐
│         MainWindow (WPF UI)             │
│  - 用户交互                              │
│  - 状态显示                              │
│  - 计时器                                │
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│    NaudioLoopbackRecorder               │
│  - WASAPI 音频捕获                       │
│  - 格式转换 (WAV/MP3)                    │
│  - 文件写入                              │
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│      Windows Audio System               │
│  - 系统音频输出流                         │
└─────────────────────────────────────────┘
```

## 📝 常见问题

### Q: 为什么录制的音频没有声音？
**A**: 请确保：
1. 电脑正在播放音频
2. 系统音量不为 0
3. 音频设备正常工作

### Q: 可以录制麦克风吗？
**A**: 当前版本仅支持系统音频环回（录制电脑播放的声音）。麦克风录制功能计划在未来版本中添加。

### Q: WAV 和 MP3 有什么区别？
**A**:
- **WAV**: 无损格式，音质最佳，文件较大（~10 MB/分钟）
- **MP3**: 压缩格式，文件较小（~1 MB/分钟），音质略有损失（128kbps）

### Q: 录音文件保存在哪里？
**A**: 默认保存在程序目录下的 `Recordings` 文件夹。点击界面底部的路径可自定义保存位置。

### Q: 可以在录制时暂停吗？
**A**: 当前版本暂不支持暂停功能，计划在未来版本中添加。

## 🗺️ 开发路线图

### 下一版本 (v1.1.0)
- [ ] 音频电平指示器
- [ ] 暂停/继续录制
- [ ] 快捷键支持 (Ctrl+R)
- [ ] 录制历史列表

### 未来计划
- [ ] 麦克风录制模式
- [ ] 多音频设备选择
- [ ] 音频效果（降噪、均衡器）
- [ ] 自动静音检测和裁剪
- [ ] 深色模式

## 🤝 贡献

欢迎贡献代码、报告问题或提出建议！

1. Fork 本仓库
2. 创建您的特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

## 📄 许可证

本项目采用 MIT 许可证 - 详见 [LICENSE](LICENSE) 文件

## 🙏 致谢

- [NAudio](https://github.com/naudio/NAudio) - 强大的 .NET 音频库
- [NAudio.Lame](https://github.com/Corey-M/NAudio.Lame) - MP3 编码支持
- [LAME](https://lame.sourceforge.io/) - 高质量 MP3 编码器

## 📧 联系方式

- **项目主页**: https://github.com/billye220670/QkRecord
- **问题反馈**: https://github.com/billye220670/QkRecord/issues

---

<div align="center">

**如果这个项目对你有帮助，请给个 ⭐️ Star 支持一下！**

Made with ❤️ by billye220670

</div>
