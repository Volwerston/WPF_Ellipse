   M S B A M L   `   `   `  ����F  BWPF_Ellipse, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:WPF_EllipseWPF_Ellipse  5   	       WPF_Ellipse.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:WPF_Ellipse     5   	   -   5          �Name$  MainWindowOne��5   	    � Title$ Ellipse Painter��6    $	��550��68   $	��550��6E   $	��750��6U   $	��750��6a   .��5	      ]� .+�5
   
   o� -   5         o�Name$ aa��5
   $   $	��Top=�6   .@�5      m� $
��File��6   .@�5      m� $	��New��6    m� Command 6(   �� *ClearWindow_Command+m� 5      $
��Open��6    6)   �� *OpenFile_Command+m� 5      $
��Save��6    6)   �� *SaveFile_Command+5      m� 5      $��Shapes��6   ��6'   �� *
Ellipses+��5      �� 5       �� 
TargetType" m�6   .�5      ��  �� Property � �� �� Value 63   �� *Name+�� 5       �@m�IsCheckable . 68   �� 5       �@m�Command 64   �� *#!DataContext.SelectEllipse_Command+ �� RelativeSource ��  �� AncestorType" ��	 �� Mode$	 FindAncestor=��� 5       �@m�CommandParameter 6=   �� 6Q   5      5      5      �� 5   
   $��Bottom=�68   ��6   �� *
Ellipses+��5      �� 5      .R�5      �� 
 ��Name$
 canvas��6    K� IsItemsHost . 
������  J� InputBindings 5      d� 5        d� Gesture$ 	LeftClicka�6+    �� Command 6?   �� *DrawClick_Command+ �� CommandParameter 6e   ��  �� ElementName$ Surface��d� 5!      $ 
RightClicka�6+    6@   �� *Drag_Command+ 6a   �� $ Surface��5#      5$      5&      5'      5(      