   M S B A M L   `   `   `  ����E  ASummarizer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:Summarizer
Summarizer  5   	       Summarizer.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:Summarizer     5   	     � Title$  
MainWindow��5   	   $��1184.529��6   $��2096.742��6.    � WindowState$ 	Maximized=�6?   .��5	      � $
��1018��6   $��10,10,18.4,0q�6   $	��Top=�6/   .+�5
   
   �� -   5        $��	Get story��5
      $��10,9,0,0q�6%    J� RenderTransformOrigin$ 0.459,0.723&�67   $��32��6p   $	��Top=�6|   $��88��6�   $
��Left=�6�   z�5      V� 5      .~�5      �� �� 5      �� 5       �� Angle$ -0.11Q�6&   T� 5       T� X$ 0.012Q�6)    T� Y$ 0.006Q�63   5      5      �� 5   
   -   5         ��Name$ 	rtxtStory��5      $��10,78,1029,10q�6)   . �5      /� .T�5      J� .*�6   .*�5      �� $� ��6   5      5      5      �� 5   
   -   5         ��Name$ btnSummarize��5      $��	Summarize��6'   $��	108,9,0,0q�6;   $��32��6i   $	��Top=�6u   $��88��6�   $
��Left=�6�   �� 5   
   -   5        $ CloudRtxBox��5      $	��443��6+   $��1038,46,10,0q�68   $	��Top=�6N   . �5      /� .T�5      J� .*�6   .*�5      �� $� ��6   5       5!      5"      �� 5#   
   -   5         ��Name$ sliderThreshold��5#      $
��Left=�6*   $��1038,525,0,0q�6E   $	��Top=�6[   $	��342��6s   	 �� TickPlacement$
	 Both=�6�   
 � LargeChange$
 0.001Q�6�    � SmallChange$ 0.001Q�6�    �� TickFrequency$ 0.0001Q�6�   �� 5$   
   -   5         ��Name$ lblThreshold��5$      $��Sentence Threshold��6&   $
��Left=�6C   $��1038,494,0,0q�6^   $	��Top=�6t   $	��183��6�   �� 5%   
   -   5         ��Name$ txtTitle��5%      $��27��6$   $��10,46,1029,0q�60    �� TextWrapping$
 Wrap=�6F   $��Title��6Z   $	��Top=�6g   5&      5'      