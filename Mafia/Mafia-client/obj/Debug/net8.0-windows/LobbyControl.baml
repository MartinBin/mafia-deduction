   M S B A M L   `   `   `  ����G  CMafia-client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:Mafia_clientMafia_client  5          Mafia_client.LobbyControl   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ,localclr-namespace:Mafia_client     5      .��5	      �   � RowDefinitions  5
   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5       � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��
Game Lobby��6   $��24(�6&   $��Center=�64   $��10q�6Q   $��2��6]   �� 5   
   -   5         ��Name$ PlayerListBox��5      $��1��6)   $��10q�66   $��0��6B   ��5      �� 5      .R�5      �� $��16(�61   ��6    �� 61   5      5      �� 5   
   $��2��6   $��10q�6"   $��0��6.   .+�5      �� -   5        $ ChatMessagesListBox��5      $��80��63   �� 5        �� Orientation$ 
Horizontal=�6   .+�5!      �� -   5         ��Name$ ChatInputTextBox��5!      $	��300��64   �� 5"      -   5        $
��Send��5"      6(   5#      5$      �� 5%   
   $��1��6   $��1��6"   $��10q�62   .+�5&      �� $��Players:��6   $��18(�6(   $��0,0,0,10q�66   �� 5'      -   5         ��Name$ PlayerCountTextBlock��5'      $��0��66   $��24(�6?   $��Center=�6M   5(      5)      5*      