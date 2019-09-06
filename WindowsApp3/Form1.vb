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

        For i = 0 To 6
            TextBox1.Text = TextBox1.Text & vbCrLf & Reader.fw_authentication(icdev, i, i)
        Next i




    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        leertarjeta()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'TODO: esta línea de código carga datos en la tabla 'DataSet1.EAD_Horario_Productivo' Puede moverla o quitarla según sea necesario.
        Me.EAD_Horario_ProductivoTableAdapter.Fill(Me.DataSet1.EAD_Horario_Productivo)
        'TODO: esta línea de código carga datos en la tabla 'DataSet1.EAD_Horario_Productivo' Puede moverla o quitarla según sea necesario.
        '  Me.EAD_Horario_ProductivoTableAdapter.Fill(Me.DataSet1.EAD_Horario_Productivo)

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
                'Completamos la tabla de entradas en la empresa
                Dim newrow As DataRow
                newrow = DataSet1.EAD_marcajes.NewRow
                newrow("Fecha") = Date.Now
                newrow("Operario") = OperariosBindingSource.Current("Operario")
                DataSet1.EAD_marcajes.Rows.Add(newrow)
                Me.EAD_marcajesTableAdapter.Update(DataSet1.EAD_marcajes)

                ''Completamos la tabla de control horario
                'Dim newrowHorario As DataRow
                'newrowHorario = DataSet1.EAD_Horario_Productivo.NewRow
                'newrowHorario("CodigoEmpresa") = 1
                'newrowHorario("ID") = Guid.NewGuid
                'newrowHorario("CodigoTarjeta") = TextBox1.Text
                'newrowHorario("CodigoOperario") = OperariosBindingSource.Current("Operario")
                'newrowHorario("NombreOperario") = OperariosBindingSource.Current("NombreOperario")
                '' Comprobamos si el operario esta dentro o fuera 
                'If OperariosBindingSource.Current("EntradaSalida") = "I" Then
                '    OperariosBindingSource.Current("EntradaSalida") = "O"
                '    newrowHorario("EntradaSalida") = "O"

                'Else
                '    OperariosBindingSource.Current("EntradaSalida") = "I"
                '    newrowHorario("EntradaSalida") = "I"
                'End If
                'OperariosBindingSource.EndEdit()
                'OperariosTableAdapter.Update(DataSet1.Operarios)
                'newrowHorario("Fecha") = Date.Now.ToString("dd/MM/yyyy")
                'newrowHorario("Hora") = Date.Now.ToString("HH:mm")
                'DataSet1.EAD_Horario_Productivo.Rows.Add(newrowHorario)
                'Me.EAD_Horario_ProductivoTableAdapter.Update(DataSet1.EAD_Horario_Productivo)

                'Comprobar si es final de jornada
                finaljornada()

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

    Sub finaljornada()
        Dim i As Integer
        'Cuando sales a las 15 o mas tarde pero haces tu horario normal
        If Date.Now.ToString("HH:mm") >= "14:59" And OperariosBindingSource.Current("Marcaje") = 3 Then
            Me.EAD_marcajesTableAdapter.FillBy(Me.DataSet1.EAD_marcajes, OperariosBindingSource.Current("Operario"), Date.Now.ToString("dd/MM/yyyy"))
            If EAD_marcajesBindingSource.Count = 4 Then
                i = MsgBox("Has hecho el horario normal?", vbOKCancel, "Horario normal")
                If i = 1 Then
                    introducirturno1()
                End If
            End If
        End If

    End Sub
    Sub introducirturno1()
        Dim i As Integer
        'Completamos la tabla de control horario
        For i = 0 To 3
            Dim newrowHorario As DataRow
            newrowHorario = DataSet1.EAD_Horario_Productivo.NewRow
            newrowHorario("CodigoEmpresa") = 1
            newrowHorario("ID") = Guid.NewGuid
            newrowHorario("CodigoTarjeta") = TextBox1.Text
            newrowHorario("CodigoOperario") = OperariosBindingSource.Current("Operario")
            newrowHorario("NombreOperario") = OperariosBindingSource.Current("NombreOperario")
            ' hacemos el traspaso del horario a la segunda tabla
            Select Case i
                Case 0
                    newrowHorario("EntradaSalida") = "E"
                    If Format(Me.EAD_marcajesBindingSource.Item(i)("Fecha"), "HH:mm") <= "07:00" Then
                        newrowHorario("Hora") = "07:00"
                        newrowHorario("ok") = 0
                    Else
                        newrowHorario("Hora") = Format(Me.EAD_marcajesBindingSource.Item(i)("Fecha"), "HH:mm")
                        newrowHorario("ok") = 1
                    End If
                Case 1
                    newrowHorario("EntradaSalida") = "S"
                    Dim horas As Long = DateDiff(DateInterval.Minute, Me.EAD_marcajesBindingSource.Item(i)("Fecha"), Me.EAD_marcajesBindingSource.Item(i + 1)("Fecha"))
                    If horas <= 31 Then
                        newrowHorario("Hora") = "09:30"
                        newrowHorario("ok") = 0
                    Else
                        newrowHorario("Hora") = Format(Me.EAD_marcajesBindingSource.Item(i)("Fecha"), "HH:mm")
                        newrowHorario("ok") = 1
                    End If
                Case 2
                    newrowHorario("EntradaSalida") = "E"
                    Dim horas As Long = DateDiff(DateInterval.Minute, Me.EAD_marcajesBindingSource.Item(i - 1)("Fecha"), Me.EAD_marcajesBindingSource.Item(i)("Fecha"))
                    If horas <= 31 Then
                        newrowHorario("Hora") = "10:00"
                        newrowHorario("ok") = 0
                    Else
                        newrowHorario("Hora") = Format(Me.EAD_marcajesBindingSource.Item(i)("Fecha"), "HH:mm")
                        newrowHorario("ok") = 1
                    End If
                Case 3
                    newrowHorario("EntradaSalida") = "S"
                    Dim horas As Long = DateDiff(DateInterval.Minute, Me.EAD_marcajesBindingSource.Item(i - 2)("Fecha"), Me.EAD_marcajesBindingSource.Item(i - 1)("Fecha"))
                    If horas <= 31 Then
                        newrowHorario("Hora") = "15:00"
                        newrowHorario("ok") = 0
                    Else
                        newrowHorario("Hora") = Format(Date.Now, "HH:mm")
                        newrowHorario("ok") = 1
                    End If


            End Select
            newrowHorario("Fecha") = Date.Now.ToString("dd/MM/yyyy")
            DataSet1.EAD_Horario_Productivo.Rows.Add(newrowHorario)
            Me.EAD_Horario_ProductivoTableAdapter.Update(DataSet1.EAD_Horario_Productivo)
        Next


    End Sub
End Class
