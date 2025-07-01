# Tab 結構重構說明

## 概述
已將原本內嵌在 `HomeView.axaml` 中的Tab內容分離成獨立的View文件，以提高代碼的可維護性和可讀性。

## 新的文件結構

### Views 目錄
```
Views/
├── HomeView.axaml              # 主要的Tab容器
├── HomeTabView.axaml           # 首頁Tab內容
├── ProfileTabView.axaml        # 個人資料Tab內容
├── SettingsTabView.axaml       # 設定Tab內容
├── HelpTabView.axaml           # 幫助Tab內容
└── [其他原有View文件...]
```

## 各Tab View的功能

### 1. HomeTabView
- 顯示歡迎訊息
- 用戶資訊顯示
- Refresh Token測試功能
- 登出功能

### 2. ProfileTabView
- 用戶頭像顯示
- 個人資料資訊
- 編輯個人資料按鈕
- 修改密碼按鈕

### 3. SettingsTabView
- 通知設定
- 隱私設定
- 語言設定
- 主題設定
- 儲存設定功能

### 4. HelpTabView
- 常見問題（可展開）
- 聯絡資訊
- 意見回饋表單

## 優勢

1. **模組化**: 每個Tab的內容都是獨立的View，便於維護
2. **可重用性**: 各個Tab View可以在其他地方重用
3. **可讀性**: 代碼結構更清晰，易於理解
4. **可擴展性**: 新增Tab或修改現有Tab內容更容易
5. **團隊協作**: 不同開發者可以同時處理不同的Tab

## 數據綁定

所有Tab View都綁定到 `MainWindowViewModel`，確保數據的一致性。

## 使用方式

在 `HomeView.axaml` 中，通過以下方式引用各個Tab View：

```xml
<views:HomeTabView/>
<views:ProfileTabView/>
<views:SettingsTabView/>
<views:HelpTabView/>
```

## 未來擴展

如果需要為某個Tab添加特定的ViewModel，可以創建對應的ViewModel類別，例如：
- `HomeTabViewModel`
- `ProfileTabViewModel`
- `SettingsTabViewModel`
- `HelpTabViewModel`

這樣可以進一步提高代碼的模組化程度。 