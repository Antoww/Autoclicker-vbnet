Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Diagnostics

Public Class Form1
    Private WithEvents TimerClick As New System.Windows.Forms.Timer()
    Private Clicking As Boolean = False
    Private ClicksPerSecond As Integer = 10 ' Par défaut, 10 clics par seconde

    ' Définition des constantes pour les événements souris
    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Sub mouse_event(dwFlags As Integer, dx As Integer, dy As Integer, cButtons As Integer, dwExtraInfo As Integer)
    End Sub

    Private Const MOUSEEVENTF_LEFTDOWN As Integer = &H2
    Private Const MOUSEEVENTF_LEFTUP As Integer = &H4

    ' HOOK clavier global
    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SetWindowsHookEx(idHook As Integer, lpfn As LowLevelKeyboardProc, hMod As IntPtr, dwThreadId As UInteger) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function UnhookWindowsHookEx(hhk As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function CallNextHookEx(hhk As IntPtr, nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Shared Function GetModuleHandle(lpModuleName As String) As IntPtr
    End Function

    Private Delegate Function LowLevelKeyboardProc(nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr

    Private Const WH_KEYBOARD_LL As Integer = 13
    Private Const WM_KEYDOWN As Integer = &H100

    Private Shared KeyboardHookID As IntPtr = IntPtr.Zero
    Private Shared HookCallback As LowLevelKeyboardProc = AddressOf KeyboardProc

    ' Fonction de callback pour le hook clavier
    Private Shared Function KeyboardProc(nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
        If nCode >= 0 AndAlso wParam = CType(WM_KEYDOWN, IntPtr) Then
            Dim vkCode As Integer = Marshal.ReadInt32(lParam)
            If vkCode = Keys.F6 Then
                ' Appelle la fonction pour démarrer/arrêter l'autoclicker
                Form1.ToggleClicker()
            End If
        End If
        Return CallNextHookEx(KeyboardHookID, nCode, wParam, lParam)
    End Function

    ' Active/désactive le hook clavier global
    Private Sub SetHook()
        KeyboardHookID = SetWindowsHookEx(WH_KEYBOARD_LL, HookCallback, GetModuleHandle(Nothing), 0)
    End Sub

    Private Sub RemoveHook()
        UnhookWindowsHookEx(KeyboardHookID)
    End Sub

    ' Fonction pour activer/désactiver l'autoclicker
    Public Shared Sub ToggleClicker()
        Form1.Clicking = Not Form1.Clicking

        If Form1.Clicking Then
            Dim interval As Integer = 1000 \ Form1.ClicksPerSecond
            Form1.TimerClick.Interval = interval
            Form1.TimerClick.Start()
        Else
            Form1.TimerClick.Stop()
        End If
    End Sub

    ' Événement du timer pour faire les clics
    Private Sub TimerClick_Tick(sender As Object, e As EventArgs) Handles TimerClick.Tick
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0)
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0)
    End Sub

    ' Démarrer/arrêter avec le bouton
    Private Sub ButtonStartStop_Click(sender As Object, e As EventArgs) Handles ButtonStartStop.Click
        ToggleClicker()
    End Sub

    ' Mise à jour du nombre de clics par seconde
    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        ClicksPerSecond = CInt(NumericUpDown1.Value)
        If Clicking Then
            TimerClick.Interval = 1000 \ ClicksPerSecond
        End If
    End Sub

    ' Initialisation de l'interface
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetHook() ' Active le hook clavier global au démarrage
        NumericUpDown1.Minimum = 1
        NumericUpDown1.Maximum = 1000
        NumericUpDown1.Value = 10
    End Sub

    ' Désactive le hook lors de la fermeture du programme
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        RemoveHook()
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Dim urlico As String = "https://www.flaticon.com/fr/icones-gratuites/clic-de-souris"

        Try
            Process.Start(New ProcessStartInfo(urlico) With {
                .UseShellExecute = True
            })
        Catch ex As Exception
            MessageBox.Show("Impossible d'ouvrir le lien demandé.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        Dim urlgit As String = "https://github.com/Antoww"

        Try
            Process.Start(New ProcessStartInfo(urlgit) With {
                .UseShellExecute = True
            })
        Catch ex As Exception
            MessageBox.Show("Impossible d'ouvrir le lien demandé.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
