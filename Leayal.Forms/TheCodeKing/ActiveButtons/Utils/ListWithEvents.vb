Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace TheCodeKing.ActiveButtons.Utils
    <Serializable>
    Friend Class ListWithEvents(Of T)
        Inherits List(Of T)
        Implements IList, IList(Of T)
        Private ReadOnly m_syncRoot As New Object()
        Private m_suppressEvents As Boolean

        Public Sub New()
        End Sub

        Public Sub New(collection As IEnumerable(Of T))
            MyBase.New(collection)
        End Sub

        Public Sub New(capacity As Integer)
            MyBase.New(capacity)
        End Sub

        Protected ReadOnly Property EventsSuppressed() As Boolean
            Get
                Return m_suppressEvents
            End Get
        End Property

#Region "IList Members"

        Public ReadOnly Property SyncRoot() As Object
            Get
                Return m_syncRoot
            End Get
        End Property

        Private Function IList_Add(value As Object) As Integer Implements IList.Add
            If TypeOf value Is T Then
                Add(DirectCast(value, T))
                Return Count - 1
            End If
            Return -1
        End Function

#End Region

#Region "IList<T> Members"

        Default Public Overridable Shadows Property Item(index As Integer) As T
            Get
                Return MyBase.Item(index)
            End Get
            Set
                SyncLock m_syncRoot
                    Dim equal As Boolean = False
                    If MyBase.Item(index) IsNot Nothing Then
                        equal = MyBase.Item(index).Equals(Value)
                    ElseIf MyBase.Item(index) Is Nothing AndAlso Value Is Nothing Then
                        equal = True
                    End If

                    If Not equal Then
                        MyBase.Item(index) = Value
                        OnItemModified(New ListItemEventArgs(index))
                    End If
                End SyncLock
            End Set
        End Property

        Public Overridable Shadows Sub Add(item As T)
            Dim count As Integer
            SyncLock m_syncRoot
                MyBase.Add(item)
                count = MyBase.Count - 1
            End SyncLock
            OnItemAdded(New ListItemEventArgs(count))
        End Sub

        Public Overridable Shadows Sub Clear()
            SyncLock m_syncRoot
                MyBase.Clear()
            End SyncLock
            OnCleared(EventArgs.Empty)
        End Sub

        Public Overridable Shadows Sub Insert(index As Integer, item As T)
            SyncLock m_syncRoot
                MyBase.Insert(index, item)
            End SyncLock
            OnItemAdded(New ListItemEventArgs(index))
        End Sub

        Public Overridable Shadows Function Remove(item As T) As Boolean
            Dim result As Boolean

            SyncLock m_syncRoot
                result = MyBase.Remove(item)
            End SyncLock

            ' raise the event only if the removal was successful
            If result Then
                OnItemRemoved(EventArgs.Empty)
            End If

            Return result
        End Function

        Public Overridable Shadows Sub RemoveAt(index As Integer)
            SyncLock m_syncRoot
                MyBase.RemoveAt(index)
            End SyncLock
            OnItemRemoved(EventArgs.Empty)
        End Sub

#End Region

        Public Event CollectionModified As EventHandler(Of ListModificationEventArgs)

        Public Event Cleared As EventHandler

        Public Event ItemAdded As EventHandler(Of EventArgs)

        Public Event ItemModified As EventHandler(Of EventArgs)

        Public Event ItemRemoved As EventHandler

        Public Event RangeAdded As EventHandler(Of ListRangeEventArgs)

        Public Event RangeRemoved As EventHandler

        Public Overridable Shadows Sub AddRange(collection As IEnumerable(Of T))
            SyncLock m_syncRoot
                InsertRange(MyBase.Count, collection)
            End SyncLock
        End Sub

        Public Overridable Shadows Sub InsertRange(index As Integer, collection As IEnumerable(Of T))
            Dim count As Integer
            SyncLock m_syncRoot
                MyBase.InsertRange(index, collection)
                count = MyBase.Count - index
            End SyncLock
            OnRangeAdded(New ListRangeEventArgs(index, count))
        End Sub

        Public Overridable Shadows Function RemoveAll(match As Predicate(Of T)) As Integer
            Dim count As Integer

            SyncLock m_syncRoot
                count = MyBase.RemoveAll(match)
            End SyncLock

            ' raise the event only if the removal was successful
            If count > 0 Then
                OnRangeRemoved(EventArgs.Empty)
            End If

            Return count
        End Function

        Public Overridable Shadows Sub RemoveRange(index As Integer, count As Integer)
            Dim listCountOld As Integer, listCountNew As Integer
            SyncLock m_syncRoot
                listCountOld = MyBase.Count
                MyBase.RemoveRange(index, count)
                listCountNew = MyBase.Count
            End SyncLock

            ' raise the event only if the removal was successful
            If listCountOld <> listCountNew Then
                OnRangeRemoved(EventArgs.Empty)
            End If
        End Sub

        Public Overridable Shadows Sub RemoveRange(collection As List(Of T))
            For i As Integer = 0 To collection.Count - 1
                Remove(collection(i))
            Next
        End Sub

        Public Sub SuppressEvents()
            m_suppressEvents = True
        End Sub

        Public Sub ResumeEvents()
            m_suppressEvents = False
        End Sub

        Protected Overridable Sub OnCleared(e As EventArgs)
            If m_suppressEvents Then
                Return
            End If

            RaiseEvent Cleared(Me, e)

            OnCollectionModified(New ListModificationEventArgs(ListModification.Cleared, -1, -1))
        End Sub

        Protected Overridable Sub OnCollectionModified(e As ListModificationEventArgs)
            If m_suppressEvents Then
                Return
            End If

            RaiseEvent CollectionModified(Me, e)
        End Sub

        Protected Overridable Sub OnItemAdded(e As ListItemEventArgs)
            If m_suppressEvents Then
                Return
            End If

            RaiseEvent ItemAdded(Me, e)

            OnCollectionModified(New ListModificationEventArgs(ListModification.ItemAdded, e.ItemIndex, 1))
        End Sub

        Protected Overridable Sub OnItemModified(e As ListItemEventArgs)
            If m_suppressEvents Then
                Return
            End If

            RaiseEvent ItemModified(Me, e)

            OnCollectionModified(New ListModificationEventArgs(ListModification.ItemModified, e.ItemIndex, 1))
        End Sub

        Protected Overridable Sub OnItemRemoved(e As EventArgs)
            If m_suppressEvents Then
                Return
            End If

            RaiseEvent ItemRemoved(Me, e)

            OnCollectionModified(New ListModificationEventArgs(ListModification.ItemRemoved, -1, 1))
        End Sub

        Protected Overridable Sub OnRangeAdded(e As ListRangeEventArgs)
            If m_suppressEvents Then
                Return
            End If

            RaiseEvent RangeAdded(Me, e)

            OnCollectionModified(New ListModificationEventArgs(ListModification.RangeAdded, e.StartIndex, e.Count))
        End Sub

        Protected Overridable Sub OnRangeRemoved(e As EventArgs)
            If m_suppressEvents Then
                Return
            End If

            RaiseEvent RangeRemoved(Me, e)

            OnCollectionModified(New ListModificationEventArgs(ListModification.RangeRemoved, -1, -1))
        End Sub
    End Class
End Namespace