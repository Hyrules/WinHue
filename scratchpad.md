                                <GradientStop Color="#FFFF0000" />
                                <GradientStop Color="#FEFFFF00" Offset="0.167" />
                                <GradientStop Color="#FE00FF00" Offset="0.333" />
                                <GradientStop Color="#FE00FFFF" Offset="0.5"   />
                                <GradientStop Color="#FE0000FF" Offset="0.667" />
                                <GradientStop Color="#FEFF00FF" Offset="0.833" />
                                <GradientStop Color="#FFFF0000" Offset="1.0"   />


            <Button Content="___" Command="{Binding MinimizeToTrayCommand}" ToolTip="{x:Static resx:GUI.MainForm_TrayButton}"/>

			                                <controls:CommandComboBox x:Name="cbListBridge" ItemsSource="{Binding ListBridges}" DisplayMemberPath="LongName" SelectedItem="{Binding SelectedBridge}" HorizontalAlignment="Right"  VerticalAlignment="Top" Width="200" Height="26" BorderThickness="0">
                                    <controls:CommandComboBox.Style>
                                        <Style TargetType="{x:Type ComboBox}">
                                            <Setter Property="Visibility" Value="Visible"/>
                                            <Setter Property="Background" Value="Transparent"/>
                                            <Setter Property="BorderBrush" Value="Transparent"/>
                                            <Style.Triggers>
                                                <DataTrigger Binding="{Binding MultiBridgeCB}" Value="False">
                                                    <Setter Property="Visibility" Value="Collapsed"/>
                                                </DataTrigger>
                                            </Style.Triggers>
                                        </Style>
                                    </controls:CommandComboBox.Style>
                                </controls:CommandComboBox>

		
