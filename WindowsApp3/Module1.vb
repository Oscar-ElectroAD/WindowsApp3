Imports System.Runtime.InteropServices

Module Module1

    Public Class Reader

        'comm function
        Declare Function fw_init Lib "umf.dll" (ByVal port As Integer, ByVal baud As Long) As Integer
        Declare Function fw_exit Lib "umf.dll" (ByVal icdev As Integer) As Integer
        Declare Function fw_request Lib "umf.dll" (ByVal icdev As Integer, ByVal mode%, ByVal tagtype As Long) As Integer
        Declare Function fw_anticoll Lib "umf.dll" (ByVal icdev As Integer, ByVal bcnt%, ByVal snr As Long) As Integer
        Declare Function fw_select Lib "umf.dll" (ByVal icdev As Integer, ByVal snr As Long, ByVal size As Byte) As Integer
        Declare Function fw_card Lib "umf.dll" (ByVal icdev As Integer, ByVal mode%, ByVal snr() As Long) As Integer
        Declare Function fw_card_hex Lib "umf.dll" (ByVal icdev As Integer, ByVal mode%, ByRef cardSn As Byte) As Integer
        Declare Function fw_card_str Lib "umf.dll" (ByVal icdev As Integer, ByVal mode%, ByRef cardSn As Byte) As Integer

        Declare Function fw_load_key Lib "umf.dll" (ByVal icdev As Integer, ByVal mode%, ByVal secnr%, ByRef nkey As Byte) As Integer
        Declare Function fw_authentication Lib "umf.dll" (ByVal icdev As Integer, ByVal mode%, ByVal scenr%) As Integer
        Declare Function fw_read Lib "umf.dll" (ByVal icdev As Integer, ByVal adr%, ByRef sdata As Byte) As Integer
        Declare Function fw_write Lib "umf.dll" (ByVal icdev As Integer, ByVal adr%, ByRef sdata As Byte) As Integer
        Declare Function fw_changeb3 Lib "umf.dll" (ByVal adr As Integer, ByVal secer As Integer, ByRef KeyA As Byte, ByRef Ctrlbytes As Byte, ByVal Bk As Integer, ByRef KeyB As Byte) As Integer



        Declare Function fw_HL_initval Lib "umf.dll" (ByVal icdev As Integer, ByVal mode As Integer, ByVal adr%, ByVal value As Long, ByRef snr As Long) As Integer
        Declare Function fw_HL_increment Lib "umf.dll" (ByVal icdev As Integer, ByVal mode As Integer, ByVal adr%, ByVal value As Long, ByVal snr As Long, ByVal value As Long, ByRef snr As Long) As Integer
        Declare Function fw_HL_decrement Lib "umf.dll" (ByVal icdev As Integer, ByVal mode As Integer, ByVal adr%, ByVal value As Long, ByVal snr As Long, ByVal value As Long, ByRef snr As Long) As Integer

        '
        Declare Function fw_initval Lib "umf.dll" (ByVal icdev As Integer, ByVal adr%, ByVal value As Long) As Integer
        Declare Function fw_readval Lib "umf.dll" (ByVal icdev As Integer, ByVal adr%, ByVal value As Long) As Integer
        Declare Function fw_increment Lib "umf.dll" (ByVal icdev As Integer, ByVal adr%, ByVal value As Long) As Integer
        Declare Function fw_decrement Lib "umf.dll" (ByVal icdev As Integer, ByVal adr%, ByVal value As Long) As Integer
        Declare Function fw_restore Lib "umf.dll" (ByVal icdev As Integer, ByVal adr%) As Integer
        Declare Function fw_transfer Lib "umf.dll" (ByVal icdev As Integer, ByVal adr%) As Integer
        Declare Function fw_halt Lib "umf.dll" (ByVal icdev As Integer) As Integer


        'device fuction
        Declare Function fw_gettime Lib "umf.dll" (ByVal icdev As Integer, ByVal ctime As Byte) As Integer
        Declare Function fw_settime Lib "umf.dll" (ByVal icdev As Integer, ByVal ctime As Byte) As Integer
        Declare Function fw_getver Lib "umf.dll" (ByVal icdev As Integer, ByRef strVer As Byte) As Integer
        Declare Function fw_GetDevSN Lib "umf.dll" (ByVal icdev As Integer, ByRef bufSN As Byte, ByRef rLen As Integer) As Integer


        Declare Function fw_reset Lib "umf.dll" (ByVal icdev As Integer, ByVal msec%) As Integer
        Declare Function fw_beep Lib "umf.dll" (ByVal icdev As Integer, ByVal time1 As Integer) As Integer

        'transfer function
        Declare Sub hex_a Lib "umf.dll" (ByRef strHex As Byte, ByRef strBytes As Byte, ByVal hexLen%)        'strHex   [out]; strBytes [in]
        Declare Function a_hex Lib "umf.dll" (ByRef strBytes As Byte, ByRef strHex As Byte, ByVal charLen%) As Integer 'strBytes [out]; strHex   [in]


        'Ultralight function
        Declare Function fw_request_ultralt Lib "umf.dll" (ByVal icdev As Integer, ByVal mode%) As Integer
        Declare Function fw_anticall_ultralt Lib "umf.dll" (ByVal icdev As Integer, ByVal snr As Long) As Integer
        Declare Function fw_select_ultralt Lib "umf.dll" (ByVal icdev As Integer, ByVal snr As Long) As Integer
        Declare Function fw_write_ultralt Lib "umf.dll" (ByVal icdev As Integer, ByVal iblk%, ByRef sdata As Byte) As Integer
        Declare Function fw_read_ultralt Lib "umf.dll" (ByVal icdev As Integer, ByVal iblk%, ByRef sdata As Byte) As Integer

    End Class


End Module


