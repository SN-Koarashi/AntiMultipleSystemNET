Imports System.Management
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Imports System
Imports System.IO
Imports System.Net
Imports System.Text
Imports Microsoft.Win32

Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        '離開並關閉執行緒
        Environment.Exit(Environment.ExitCode)
        Application.Exit()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim response As MsgBoxResult
        response = MsgBox("開始之前請先閱讀下列事項" & vbCrLf & "您的物理資料：MAC地址、IP位置、硬碟名稱、硬碟序號、CPU序號" & vbCrLf & "都將透過DISCORD API傳送到遠端伺服器，作為資料驗證用途，是否同意？", vbYesNo + vbExclamation, "AMS.NET 機密資料傳送警告")

        If response = MsgBoxResult.No Then
            MsgBox("程式即將關閉", vbExclamation, "AMS.NET 訊息")
            '離開並關閉執行緒
            Environment.Exit(Environment.ExitCode)
            Application.Exit()
            Exit Sub
        End If

        Dim milliseconds = CLng(Date.UtcNow.Subtract(New DateTime(1970, 1, 1)).TotalMilliseconds)
        Dim seconds As Integer = milliseconds / 1000
        Dim OurKey = Registry.CurrentUser
        OurKey = OurKey.OpenSubKey("SOFTWARE\AntiMultipleSystemNET", True)

        If OurKey IsNot Nothing Then
            If seconds - OurKey.GetValue("timestamp") < 15 Then
                MsgBox("間隔太短，無法建立連線函數", vbExclamation, "AMS.NET 訊息")
                '離開並關閉執行緒
                Environment.Exit(Environment.ExitCode)
                Application.Exit()
            End If
        End If




        Shell("cmd.exe /c reg add hkcu\software\AntiMultipleSystemNET /v timestamp /t REG_SZ /d " & seconds & " /f", vbHide)

        Dim cmicWmi As New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive")
        Dim diskId As String '硬碟ID
        Dim diskSerialNumber As String '硬碟序號
        Dim diskModel As String '硬碟名稱
        Dim Wmi As New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_Processor")
        Dim cpuId As String 'CPU序號

        Dim netid As String = "" 'MAC Address
        Dim ip As String ' IP Address
        Dim searcher As New ManagementObjectSearcher("select * from win32_NetworkAdapterConfiguration")
        Dim moc2 As ManagementObjectCollection = searcher.Get()
        For Each cmicWmiObj As ManagementObject In cmicWmi.Get
            diskId = cmicWmiObj("signature")
            diskSerialNumber = cmicWmiObj("serialnumber")
            diskModel = cmicWmiObj("Model")
        Next

        For Each WmiObj As ManagementObject In Wmi.Get
            cpuId = WmiObj("ProcessorId")
        Next

        For Each mo As ManagementObject In moc2
            If mo("IPEnabled") Then
                netid = mo("MACAddress")
                ip = mo("IpAddress")(0)
                Exit For
            End If
        Next

        Dim result As String
        result = "==START== MAC: " & netid & ", IP: " & ip & ", HWID: [" & diskId & ", " & diskModel & ", " & diskSerialNumber & "], CPUID: " & cpuId & " ===END==="

        ' 建立連線物件
        Dim web As New System.Net.WebClient()

        ' 加入連線型態
        web.Headers.Add("Content-Type", "application/json")

        ' 加入傳送內容並編碼
        Dim d As Byte() = System.Text.Encoding.UTF8.GetBytes("{" & Chr(34) & "content" & Chr(34) & ":" & Chr(34) & result & Chr(34) & "}")

        ' 指定URL、傳送方式、byte array
        Dim res As Byte() = web.UploadData("https://discord.com/api/webhooks/899649467601551401/JthQDOrZfFImMZ8FyMQI9pC0YZYTsdUMHoptaiOnVkMu61mON52KHgWU4uEB-9qa6-MV", "POST", d)

        ' 取得回傳資料
        MsgBox("傳送成功", vbExclamation, "AMS.NET 訊息")

        '離開並關閉執行緒
        Environment.Exit(Environment.ExitCode)
        Application.Exit()
    End Sub
End Class
