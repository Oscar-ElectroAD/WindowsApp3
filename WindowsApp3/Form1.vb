Imports System.Text
Imports WindowsApp3.Module1
Imports System.Runtime.InteropServices

Public Class Form1
    Dim snr, tagtype As Long

    Dim data32(17) As Byte
    Dim databuff32(17) As Byte
    Dim databuff32_hex(33) As Byte
    Dim sector As Integer
    Dim block As Integer
    Dim cardtype As Integer 'card type 0:M1,1:S70
    Dim icdev As Integer
    Dim ihdevs(10) As Integer 'Store the handle list of readers



    Private Sub btn_getcardUID_Click(sender As Object, e As EventArgs) Handles btn_getcardUID.Click
        Dim st As Integer
        Dim cardbuf(100) As Byte
        Dim strCard As String
        Dim cardmode As Integer

        cardmode = 1

        st = Reader.fw_card_str(icdev, cardmode, cardbuf(0)) 'when cardmode=1,can repeat to find a card,otherwise must take away the card before redo when 0

        If st <> 0 Then
            TextBox1.Text = TextBox1.Text & "NO Found" & vbCrLf
            Exit Sub
        End If

        TextBox1.Text = TextBox1.Text & vbCrLf & "Find Card"
        strCard = System.Text.Encoding.Default.GetString(cardbuf) + " "
        TextBox1.Text = TextBox1.Text & vbCrLf & strCard
        Reader.fw_beep(icdev, 10)
        Me.OperariosTableAdapter.FillByNFC(Me.DataSet1.Operarios, strCard)
        Dim newrow As DataRow
        newrow = DataSet1.EAD_marcajes.NewRow
        newrow("Fecha") = Date.Now
        newrow("Operario") = OperariosBindingSource.Current("Operario")
        DataSet1.EAD_marcajes.Rows.Add(newrow)
        Me.EAD_marcajesTableAdapter.Update(DataSet1.EAD_marcajes)




    End Sub

    Private Sub btn_read_Click(sender As Object, e As EventArgs) Handles btn_read.Click
        'Dim st As Integer
        Dim databuf(16) As Byte
        Dim strRead() As Byte = System.Text.Encoding.ASCII.GetBytes("1234567890123456")
        Dim i As Integer
        Dim password() As Byte = System.Text.Encoding.ASCII.GetBytes("ffffffffffff")
        Dim byteValue2 As String = System.Text.Encoding.Default.GetString(password)
        'Dim numbers As String = 
        ' password = {ff, ff, ff, ff, ff, ff}
        ' UpdateCardReadPara()
        'password = 
        '  Dim red() As Byte = {ff, ff, ff, "ff", "ff", "ff"}
        ' Dim red2 As String = System.Text.Encoding.Default.GetString(red)
        For i = 0 To 6
            '  TextBox1.Text = TextBox1.Text & vbCrLf & Reader.fw_load_key(icdev, 4, i, red2)  'verify key

            TextBox1.Text = TextBox1.Text & vbCrLf & Reader.fw_authentication(icdev, i, i)
            'For j = 1 To 16
            '    ' Reader.fw_write(icdev, 0, strRead(j))
            '    TextBox1.Text = TextBox1.Text &' vbCrLf & Reader.fw_read(icdev, 0, databuf(j))
            '    TextBox1.Update()
            'Next j

        Next i


        'If st <> 0 Then
        '    List1.Items.Add("Call fw_authentication() function error. Confirm that do find card before!")
        '    ' Exit Sub
        'End If
        'List1.Items.Add("Call fw_authentication() function success!")

        'st = Reader.fw_read(icdev, block, databuf(0))   'read card
        'If st <> 0 Then
        '    List1.Items.Add("Call fw_read function error")
        '    'Exit Sub

        'End If
        'List1.Items.Add("Call fw_read function success")

        'If (RadioBtn_Hex.Checked() = True) Then
        '    'read as hex string
        '    Call Reader.hex_a(databuff32_hex(0), databuf(0), 32)
        '    strRead = System.Text.Encoding.Default.GetString(databuff32_hex)
        'Else
        '    'read as ASC string
        '    strRead = System.Text.Encoding.Default.GetString(databuf)
        'End If

        'Txt_data.Text = strRead

    End Sub

    Private Sub OperariosBindingNavigatorSaveItem_Click(sender As Object, e As EventArgs)
        Me.Validate()
        Me.OperariosBindingSource.EndEdit()
        Me.TableAdapterManager.UpdateAll(Me.DataSet1)

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        leertarjeta()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim tmphdev As Integer
        Dim i As Integer
        Dim strdev As String
        Dim st As Integer
        Dim snbuf(100) As Byte
        Dim szSN(100) As Byte
        Dim strSN As String
        Dim lenbuf(2) As Integer
        icdev = -1
        cardtype = 0
        i = 0

        Do
            tmphdev = Reader.fw_init(100, 0)

            strdev = "Reader"

            If tmphdev <> -1 Then
                ihdevs(i) = tmphdev
                strdev = strdev + Str(i)

                st = Reader.fw_GetDevSN(tmphdev, snbuf(0), lenbuf(0))
                If st = 0 Then
                    Call Reader.hex_a(szSN(0), snbuf(0), 2 * lenbuf(0))
                    strSN = System.Text.Encoding.Default.GetString(szSN)
                    TextBox1.Text = "DevSN:" & strSN & vbCrLf
                End If

            End If

            i = i + 1

        Loop Until tmphdev < 0 And i < 100
        icdev = ihdevs(0)
        Timer1.Start()

    End Sub
    Sub leertarjeta()
        Dim st As Integer
        Dim cardbuf(100) As Byte
        Dim strCard As String
        Dim cardmode As Integer

        cardmode = 1
        Label1.Text = Date.Now.ToString("HH:mm")
        st = Reader.fw_card_str(icdev, cardmode, cardbuf(0)) 'when cardmode=1,can repeat to find a card,otherwise must take away the card before redo when 0

        If st <> 0 Then
            ' TextBox1.Text = TextBox1.Text & "NO Found" & vbCrLf

        Else

            'TextBox1.Text = TextBox1.Text & vbCrLf & "Find Card"
            strCard = System.Text.Encoding.Default.GetString(cardbuf) + " "
                TextBox1.Text = strCard
                Reader.fw_beep(icdev, 10)
            Me.OperariosTableAdapter.FillByNFC(Me.DataSet1.Operarios, strCard)
            Try
                Me.EAD_marcajesTableAdapter.Fillby5min(Me.DataSet1.EAD_marcajes, Date.Now.AddMinutes(-6), Date.Now, OperariosBindingSource.Current("Operario"))
            Catch ex As Exception

                Label3.Visible = True
                Label3.Text = "ERROR TARJETA NO RELACIONADA"
                Label3.Update()

                System.Threading.Thread.Sleep(5000)
                Label3.Visible = False
                Exit Sub
            End Try
            If EAD_marcajesBindingSource.Count = 0 Then
                    Dim newrow As DataRow
                    newrow = DataSet1.EAD_marcajes.NewRow
                    newrow("Fecha") = Date.Now
                    newrow("Operario") = OperariosBindingSource.Current("Operario")
                    DataSet1.EAD_marcajes.Rows.Add(newrow)
                    Me.EAD_marcajesTableAdapter.Update(DataSet1.EAD_marcajes)
                    Label3.Visible = True
                    Label3.Text = OperariosBindingSource.Current("NombreOperario")
                    Label3.Update()
                    System.Threading.Thread.Sleep(1000)
                    Label3.Visible = False
                Else
                    Label2.Visible = True
                    Label2.Update()
                    System.Threading.Thread.Sleep(1000)
                    Label2.Visible = False
                End If
            ' TextBox1.Text = EAD_marcajesBindingSource.Current("Operario") & " " & EAD_marcajesBindingSource.Current("fecha")

        End If
    End Sub


End Class
