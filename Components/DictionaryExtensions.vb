Public Module DictionaryExtensions

 <System.Runtime.CompilerServices.Extension()> _
 Public Function GetValue(Of T)(bag As IDictionary, key As Object, defaultValue As T) As T
  Try
   Return CType(bag(key), T)
  Catch ex As Exception
   Return defaultValue
  End Try
 End Function

End Module
