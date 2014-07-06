Module Extensions

 <System.Runtime.CompilerServices.Extension()>
 Public Function NormalizeString(input As String) As String
  Dim enc As Encoding = Encoding.GetEncoding("iso-8859-8")
  Return enc.GetString(Encoding.Convert(Encoding.UTF8, enc, Encoding.UTF8.GetBytes(input))).ToLower
 End Function


End Module
