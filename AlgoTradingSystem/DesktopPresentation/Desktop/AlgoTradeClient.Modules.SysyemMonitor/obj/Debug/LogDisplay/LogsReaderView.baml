   M S B A M L   `   `   `  ����'  #StockTraderRI.Modules.SystemMonitorE    ?AlgoTradeClient.Modules.SystemMonitor.LogDisplay.LogsReaderView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      -   5          >�Name$  
newsReader��5   1   $
��Auto��6   $
��Auto��6   $��0,0,0,0q�6     � 	Resources 5       
  FadeIn g� 	BeginTime �� 
TargetName �� TargetProperty O� KeyTime O� Value��%�   &	        �� 5   
   .��5      U� $ 00:00:00X�6,   $ 	logReader��6A   $ (UIElement.Opacity)�6c   .\�5      �� $ 00:00:00��6'   $ 0Q�6:   �� 5	      $ 00:00:00.5000000��6'   $ 1Q�6B   5
      5      5       � Triggers 5      :� 5   
    :� RoutedEvent$ FrameworkElement.Loaded��6   .[�5      �� #��[  6   5      .��5      �� $��1q�6   	 �� Padding$	 5q�6!   ���	#FFFFFFFF._�5   
   � 
 � RowDefinitions
 5      �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $��* �6    .+�5      �� $��0��6   $��5q�6%   $
��Auto��60   $��11(�6>   $
��Bold"�6L   
�����F� �� TextAlignment$
 Left=�6u   $
��Wrap=�6�   $
��Left=�6�   $
��Auto��6�   ��6�   �� *LogsList.PublishedDate+6�   �� 5      $��1��6   $
��Auto��6%   $��5q�64   $��18(�6?   $��Normal"�6M   
�����F�$
 Left=�6x   $
��Wrap=�6�   ��6�   ��  �� Path$ NewsArticle.Title�6�   �� 5      $��1��6   
������ ���	#FFFFFFFF$��0,0,0,0q�6T   $��12(�6n   $��Normal$�6|   $��Normal"�5      ���	#FF000000 J� Opacity$ 1Q�6E    �� TextAlignment$
 Left=�5       �� TextWrapping$
 Wrap=�6/    �� 
LineHeight$
 Auto��6C   $
��Left=�6[   $	��Top=�6v     PopupTextBoxStyle#��[ 5       �� 
IsReadOnly .  J� OpacityMask 5   Q   Z� ��6h   �� *NewsArticle.Body+5   E   5      5       5!      