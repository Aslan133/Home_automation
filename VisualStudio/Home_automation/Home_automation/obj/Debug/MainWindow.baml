   M S B A M L   `   `   `  ����J  FHome_automation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:Home_automationHome_automation  5   	        Home_automation.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:Home_automation     5   	   -   5          � Title$  HOME AUTOMATION��5   	   $��328.667��6!   $	��400��62    � 
ResizeMode$ CanMinimize=�6>   ���	#FFD3F5FF
���   ���.  � WindowStyle$
 None=�6�   ���	#FF0017FF$��1q�6�    � Icon$ Icon.PNG��6�    � 	Resources 5	         ButtonControlTemplate1 �� 
TargetType ��Name J� SnapsToDevicePixels ��Name	 �� ContentStringFormat
 �� ContentStringFormat �� Triggers P� Property P� Value �� 
TargetName �� Value �� Property��%�  &	        �� 5
   
   " ��69   .R�5      �� $ border��6   #��z��6%   #��z��6Q   #��z��6�    . ._�5      �� $ contentPresenter��6#   #��z��6=   #��z��6q   #
 z	 6�   |�.  #��z��6�   #��z��6&  ��. # z 6d  #��z��6�  5       5      P� 5       �@��IsDefaulted .  . �5      �� $ border��64   # � ��6H    � ��5      P� 5       �@J�IsMouseOver . . �5      �� $ border��60    �@J�Opacity$	 0.5Q�6D   5      P� 5       � �� . . �5      �� $ border��60    �@J�Opacity$	 0.5Q�6D   5      P� 5       �@d�	IsChecked$
 TrueY�6<   . �5      �� $ border��63    � �� �	#FFBCDDEE6G   �� 5      $ border��64    � �� �	#FF245A836H   5      P� 5       � {� .  . �5      �� $ border��63    � �� �	#FFF4F4F46G   �� 5      $ border��64    � �� �	#FFADB2B56H   �� 5       � �� �	#FF83838363   5       5"      5#      .��5$      � .+�5%   
   ��  �� Orientation$ 
Horizontal=�6   $��35��6.   $	��Top=�6:   $��0,0,86,0q�6R   .+�5&      �� -   5         ��Name$ 
MainNavBtn��5&      $
��MAIN��6)   $��70��68   $��0,0,0,0q�6F   ���	#FF98E6FD$
��Bold"�6�   $��16(�6�   ���	#FF0023FF6�   �� 5'      -   5        $ GraphNavBtn��5'      $��GRAPH��6*   $��70��6:   $��5,0,0,0q�6H   ���	#FF98E6FD$��16(�6�   $
��Bold"�6�   ���	#FF0023FF6�   �� 5(      -   5        $ ErrorsNavBtn��5(      $��ERRORS��6+   $��70��6<   $��5,0,0,0q�6J   ���	#FF98E6FD$
��Bold"�6�   $��16(�6�   ���	#FF0023FF6�   5)       � 5+   
   -   5          �Name$
 Main��5+      $��0,35,0,0q�6     � NavigationUIVisibility$ Hidden=�60   
����������	#FF3D61FF$��0q�6{   $��0,1,0,0q�6�   �� 5,   
   -   5        $ ExitBtn��5,      $�� ��6"   $
��Left=�6-   $��	363,0,0,0q�6H   $	��Top=�6[   $��37��6s   $��35��6~   #���   6�   ��5.      �� 5/       �� ImageSource$ ExitBtn.PNG��6    j� TileMode$ FlipX=�67   52      �� 53   
   -   5        $ MinimizeBtn��53      $�� ��6&   $
��Left=�61   $��	323,0,0,0q�6L   $	��Top=�6_   $��37��6w   $��35��6�   #���   6�   $��24(�6�   $
��Bold"�6�   ��54      �� 55      $ MinimizeBtn.PNG��6   57      59      5:      