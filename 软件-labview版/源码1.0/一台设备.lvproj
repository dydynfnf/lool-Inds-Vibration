<?xml version='1.0' encoding='UTF-8'?>
<Project Type="Project" LVVersion="12008004">
	<Item Name="我的电脑" Type="My Computer">
		<Property Name="server.app.propertiesEnabled" Type="Bool">true</Property>
		<Property Name="server.control.propertiesEnabled" Type="Bool">true</Property>
		<Property Name="server.tcp.enabled" Type="Bool">false</Property>
		<Property Name="server.tcp.port" Type="Int">0</Property>
		<Property Name="server.tcp.serviceName" Type="Str">我的电脑/VI服务器</Property>
		<Property Name="server.tcp.serviceName.default" Type="Str">我的电脑/VI服务器</Property>
		<Property Name="server.vi.callsEnabled" Type="Bool">true</Property>
		<Property Name="server.vi.propertiesEnabled" Type="Bool">true</Property>
		<Property Name="specify.custom.address" Type="Bool">false</Property>
		<Item Name="64X64.ico" Type="Document" URL="../64X64.ico"/>
		<Item Name="ipconfig.vi" Type="VI" URL="../ipconfig.vi"/>
		<Item Name="恒流源控制.vi" Type="VI" URL="../恒流源控制.vi"/>
		<Item Name="一台设备.vi" Type="VI" URL="../一台设备.vi"/>
		<Item Name="自校准.vi" Type="VI" URL="../自校准.vi"/>
		<Item Name="依赖关系" Type="Dependencies">
			<Item Name="vi.lib" Type="Folder">
				<Item Name="NI_AALPro.lvlib" Type="Library" URL="/&lt;vilib&gt;/Analysis/NI_AALPro.lvlib"/>
				<Item Name="Space Constant.vi" Type="VI" URL="/&lt;vilib&gt;/dlg_ctls.llb/Space Constant.vi"/>
			</Item>
			<Item Name="lvanlys.dll" Type="Document" URL="/&lt;resource&gt;/lvanlys.dll"/>
		</Item>
		<Item Name="程序生成规范" Type="Build">
			<Item Name="安装程序" Type="Installer">
				<Property Name="Destination[0].name" Type="Str">振动测试仪</Property>
				<Property Name="Destination[0].parent" Type="Str">{3912416A-D2E5-411B-AFEE-B63654D690C0}</Property>
				<Property Name="Destination[0].tag" Type="Str">{0B10E76F-6CAA-4F25-B45F-74FFFEA34033}</Property>
				<Property Name="Destination[0].type" Type="Str">userFolder</Property>
				<Property Name="DestinationCount" Type="Int">1</Property>
				<Property Name="DistPart[0].flavorID" Type="Str">DefaultFull</Property>
				<Property Name="DistPart[0].productID" Type="Str">{5157CC53-EB17-4E69-A5C9-73E5695198B1}</Property>
				<Property Name="DistPart[0].productName" Type="Str">NI LabVIEW运行引擎2012 SP1 f3</Property>
				<Property Name="DistPart[0].upgradeCode" Type="Str">{20385C41-50B1-4416-AC2A-F7D6423A9BD6}</Property>
				<Property Name="DistPartCount" Type="Int">1</Property>
				<Property Name="INST_author" Type="Str">lool</Property>
				<Property Name="INST_autoIncrement" Type="Bool">true</Property>
				<Property Name="INST_buildLocation" Type="Path">../builds/安装程序</Property>
				<Property Name="INST_buildLocation.type" Type="Str">relativeToCommon</Property>
				<Property Name="INST_buildSpecName" Type="Str">安装程序</Property>
				<Property Name="INST_defaultDir" Type="Str">{0B10E76F-6CAA-4F25-B45F-74FFFEA34033}</Property>
				<Property Name="INST_language" Type="Int">2052</Property>
				<Property Name="INST_productName" Type="Str">振动测试仪</Property>
				<Property Name="INST_productVersion" Type="Str">1.0.4</Property>
				<Property Name="InstSpecBitness" Type="Str">32-bit</Property>
				<Property Name="InstSpecVersion" Type="Str">12008024</Property>
				<Property Name="MSI_arpCompany" Type="Str">lool</Property>
				<Property Name="MSI_arpContact" Type="Str">psai_don@foxmail.com</Property>
				<Property Name="MSI_arpPhone" Type="Str">18602549037</Property>
				<Property Name="MSI_distID" Type="Str">{0612CCF8-5D51-44D4-B7E3-BB8667FF9D0F}</Property>
				<Property Name="MSI_osCheck" Type="Int">0</Property>
				<Property Name="MSI_upgradeCode" Type="Str">{EDE557A3-6B36-43B0-8530-1E9570A214B7}</Property>
				<Property Name="MSI_windowMessage" Type="Str">欢迎使用振动测试仪监控软件。</Property>
				<Property Name="MSI_windowTitle" Type="Str">欢迎</Property>
				<Property Name="RegDest[0].dirName" Type="Str">Software</Property>
				<Property Name="RegDest[0].dirTag" Type="Str">{DDFAFC8B-E728-4AC8-96DE-B920EBB97A86}</Property>
				<Property Name="RegDest[0].parentTag" Type="Str">2</Property>
				<Property Name="RegDestCount" Type="Int">1</Property>
				<Property Name="Source[0].dest" Type="Str">{0B10E76F-6CAA-4F25-B45F-74FFFEA34033}</Property>
				<Property Name="Source[0].File[0].dest" Type="Str">{0B10E76F-6CAA-4F25-B45F-74FFFEA34033}</Property>
				<Property Name="Source[0].File[0].name" Type="Str">恒流源控制.exe</Property>
				<Property Name="Source[0].File[0].Shortcut[0].destIndex" Type="Int">0</Property>
				<Property Name="Source[0].File[0].Shortcut[0].name" Type="Str">恒流源控制</Property>
				<Property Name="Source[0].File[0].Shortcut[0].subDir" Type="Str">振动测试仪</Property>
				<Property Name="Source[0].File[0].ShortcutCount" Type="Int">1</Property>
				<Property Name="Source[0].File[0].tag" Type="Str">{AF5BC059-4A29-4A1C-B1FA-BB0B630C3E89}</Property>
				<Property Name="Source[0].FileCount" Type="Int">1</Property>
				<Property Name="Source[0].name" Type="Str">恒流源控制</Property>
				<Property Name="Source[0].tag" Type="Ref">/我的电脑/程序生成规范/恒流源控制</Property>
				<Property Name="Source[0].type" Type="Str">EXE</Property>
				<Property Name="Source[1].dest" Type="Str">{0B10E76F-6CAA-4F25-B45F-74FFFEA34033}</Property>
				<Property Name="Source[1].File[0].dest" Type="Str">{0B10E76F-6CAA-4F25-B45F-74FFFEA34033}</Property>
				<Property Name="Source[1].File[0].name" Type="Str">主程序.exe</Property>
				<Property Name="Source[1].File[0].Shortcut[0].destIndex" Type="Int">0</Property>
				<Property Name="Source[1].File[0].Shortcut[0].name" Type="Str">主程序</Property>
				<Property Name="Source[1].File[0].Shortcut[0].subDir" Type="Str">振动测试仪</Property>
				<Property Name="Source[1].File[0].Shortcut[1].destIndex" Type="Int">1</Property>
				<Property Name="Source[1].File[0].Shortcut[1].name" Type="Str">测试仪主程序</Property>
				<Property Name="Source[1].File[0].Shortcut[1].subDir" Type="Str"></Property>
				<Property Name="Source[1].File[0].ShortcutCount" Type="Int">2</Property>
				<Property Name="Source[1].File[0].tag" Type="Str">{1CA663FA-073B-4CF1-925C-41DAF4D35297}</Property>
				<Property Name="Source[1].FileCount" Type="Int">1</Property>
				<Property Name="Source[1].name" Type="Str">主程序</Property>
				<Property Name="Source[1].tag" Type="Ref">/我的电脑/程序生成规范/主程序</Property>
				<Property Name="Source[1].type" Type="Str">EXE</Property>
				<Property Name="Source[2].dest" Type="Str">{0B10E76F-6CAA-4F25-B45F-74FFFEA34033}</Property>
				<Property Name="Source[2].File[0].dest" Type="Str">{0B10E76F-6CAA-4F25-B45F-74FFFEA34033}</Property>
				<Property Name="Source[2].File[0].name" Type="Str">装置IP配置.exe</Property>
				<Property Name="Source[2].File[0].Shortcut[0].destIndex" Type="Int">0</Property>
				<Property Name="Source[2].File[0].Shortcut[0].name" Type="Str">装置IP配置</Property>
				<Property Name="Source[2].File[0].Shortcut[0].subDir" Type="Str">振动测试仪</Property>
				<Property Name="Source[2].File[0].ShortcutCount" Type="Int">1</Property>
				<Property Name="Source[2].File[0].tag" Type="Str">{2C11B221-A698-4EBA-8C82-A4D88BED4DF5}</Property>
				<Property Name="Source[2].FileCount" Type="Int">1</Property>
				<Property Name="Source[2].name" Type="Str">装置IP配置</Property>
				<Property Name="Source[2].tag" Type="Ref">/我的电脑/程序生成规范/装置IP配置</Property>
				<Property Name="Source[2].type" Type="Str">EXE</Property>
				<Property Name="Source[3].dest" Type="Str">{0B10E76F-6CAA-4F25-B45F-74FFFEA34033}</Property>
				<Property Name="Source[3].File[0].dest" Type="Str">{0B10E76F-6CAA-4F25-B45F-74FFFEA34033}</Property>
				<Property Name="Source[3].File[0].name" Type="Str">自校准.exe</Property>
				<Property Name="Source[3].File[0].Shortcut[0].destIndex" Type="Int">0</Property>
				<Property Name="Source[3].File[0].Shortcut[0].name" Type="Str">自校准</Property>
				<Property Name="Source[3].File[0].Shortcut[0].subDir" Type="Str">振动测试仪</Property>
				<Property Name="Source[3].File[0].ShortcutCount" Type="Int">1</Property>
				<Property Name="Source[3].File[0].tag" Type="Str">{5E695CF2-F901-4ABC-A479-1E930EE62460}</Property>
				<Property Name="Source[3].FileCount" Type="Int">1</Property>
				<Property Name="Source[3].name" Type="Str">自校准</Property>
				<Property Name="Source[3].tag" Type="Ref">/我的电脑/程序生成规范/自校准</Property>
				<Property Name="Source[3].type" Type="Str">EXE</Property>
				<Property Name="SourceCount" Type="Int">4</Property>
			</Item>
			<Item Name="恒流源控制" Type="EXE">
				<Property Name="App_copyErrors" Type="Bool">true</Property>
				<Property Name="App_INI_aliasGUID" Type="Str">{99F3BBBC-B52B-4485-B4E3-8D9F9E1AA70E}</Property>
				<Property Name="App_INI_GUID" Type="Str">{56DCEBBA-31A4-400F-B44A-C4D6BC9E0A7F}</Property>
				<Property Name="App_winsec.description" Type="Str">http://www.lool.com</Property>
				<Property Name="Bld_buildCacheID" Type="Str">{B184C420-A610-4BAF-AE78-7AF21ABBD21A}</Property>
				<Property Name="Bld_buildSpecName" Type="Str">恒流源控制</Property>
				<Property Name="Bld_defaultLanguage" Type="Str">ChineseS</Property>
				<Property Name="Bld_excludeInlineSubVIs" Type="Bool">true</Property>
				<Property Name="Bld_excludeLibraryItems" Type="Bool">true</Property>
				<Property Name="Bld_excludePolymorphicVIs" Type="Bool">true</Property>
				<Property Name="Bld_localDestDir" Type="Path">../builds/恒流源控制</Property>
				<Property Name="Bld_localDestDirType" Type="Str">relativeToCommon</Property>
				<Property Name="Bld_modifyLibraryFile" Type="Bool">true</Property>
				<Property Name="Bld_previewCacheID" Type="Str">{3ABE168C-776C-4773-BDD6-C3E7F532888D}</Property>
				<Property Name="Destination[0].destName" Type="Str">恒流源控制.exe</Property>
				<Property Name="Destination[0].path" Type="Path">../builds/恒流源控制/恒流源控制.exe</Property>
				<Property Name="Destination[0].preserveHierarchy" Type="Bool">true</Property>
				<Property Name="Destination[0].type" Type="Str">App</Property>
				<Property Name="Destination[1].destName" Type="Str">支持目录</Property>
				<Property Name="Destination[1].path" Type="Path">../builds/恒流源控制/data</Property>
				<Property Name="DestinationCount" Type="Int">2</Property>
				<Property Name="Source[0].itemID" Type="Str">{5EF75751-FBB0-4288-96C0-F690C7468C45}</Property>
				<Property Name="Source[0].type" Type="Str">Container</Property>
				<Property Name="Source[1].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[1].itemID" Type="Ref">/我的电脑/恒流源控制.vi</Property>
				<Property Name="Source[1].sourceInclusion" Type="Str">TopLevel</Property>
				<Property Name="Source[1].type" Type="Str">VI</Property>
				<Property Name="SourceCount" Type="Int">2</Property>
				<Property Name="TgtF_companyName" Type="Str">lool</Property>
				<Property Name="TgtF_fileDescription" Type="Str">恒流源控制</Property>
				<Property Name="TgtF_fileVersion.major" Type="Int">1</Property>
				<Property Name="TgtF_internalName" Type="Str">恒流源控制</Property>
				<Property Name="TgtF_legalCopyright" Type="Str">版权 2016 lool</Property>
				<Property Name="TgtF_productName" Type="Str">恒流源控制</Property>
				<Property Name="TgtF_targetfileGUID" Type="Str">{AF5BC059-4A29-4A1C-B1FA-BB0B630C3E89}</Property>
				<Property Name="TgtF_targetfileName" Type="Str">恒流源控制.exe</Property>
			</Item>
			<Item Name="主程序" Type="EXE">
				<Property Name="App_copyErrors" Type="Bool">true</Property>
				<Property Name="App_INI_aliasGUID" Type="Str">{CC1D0929-5EFA-442A-884F-3E792904F798}</Property>
				<Property Name="App_INI_GUID" Type="Str">{77C17788-3480-46C0-8C18-1F9ECB1A6A7E}</Property>
				<Property Name="Bld_buildCacheID" Type="Str">{867890BB-B04D-4BC3-8682-4CBC1DFF69EB}</Property>
				<Property Name="Bld_buildSpecName" Type="Str">主程序</Property>
				<Property Name="Bld_defaultLanguage" Type="Str">ChineseS</Property>
				<Property Name="Bld_excludeInlineSubVIs" Type="Bool">true</Property>
				<Property Name="Bld_excludeLibraryItems" Type="Bool">true</Property>
				<Property Name="Bld_excludePolymorphicVIs" Type="Bool">true</Property>
				<Property Name="Bld_localDestDir" Type="Path">../builds/主程序</Property>
				<Property Name="Bld_localDestDirType" Type="Str">relativeToCommon</Property>
				<Property Name="Bld_modifyLibraryFile" Type="Bool">true</Property>
				<Property Name="Bld_previewCacheID" Type="Str">{40D4E334-111A-4752-AF12-062CFBC8F8F0}</Property>
				<Property Name="Destination[0].destName" Type="Str">主程序.exe</Property>
				<Property Name="Destination[0].path" Type="Path">../builds/主程序/主程序.exe</Property>
				<Property Name="Destination[0].preserveHierarchy" Type="Bool">true</Property>
				<Property Name="Destination[0].type" Type="Str">App</Property>
				<Property Name="Destination[1].destName" Type="Str">支持目录</Property>
				<Property Name="Destination[1].path" Type="Path">../builds/主程序/data</Property>
				<Property Name="DestinationCount" Type="Int">2</Property>
				<Property Name="Exe_iconItemID" Type="Ref">/我的电脑/64X64.ico</Property>
				<Property Name="Source[0].itemID" Type="Str">{439DA2C1-7C8F-4F2A-86CB-E8FB10D5E749}</Property>
				<Property Name="Source[0].type" Type="Str">Container</Property>
				<Property Name="Source[1].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[1].itemID" Type="Ref">/我的电脑/一台设备.vi</Property>
				<Property Name="Source[1].sourceInclusion" Type="Str">TopLevel</Property>
				<Property Name="Source[1].type" Type="Str">VI</Property>
				<Property Name="Source[2].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[2].itemID" Type="Ref">/我的电脑/恒流源控制.vi</Property>
				<Property Name="Source[2].type" Type="Str">VI</Property>
				<Property Name="Source[3].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[3].itemID" Type="Ref">/我的电脑/ipconfig.vi</Property>
				<Property Name="Source[3].properties[0].type" Type="Str">Window has title bar</Property>
				<Property Name="Source[3].properties[0].value" Type="Bool">false</Property>
				<Property Name="Source[3].properties[1].type" Type="Str">Show menu bar</Property>
				<Property Name="Source[3].properties[1].value" Type="Bool">false</Property>
				<Property Name="Source[3].properties[2].type" Type="Str">Show vertical scroll bar</Property>
				<Property Name="Source[3].properties[2].value" Type="Bool">true</Property>
				<Property Name="Source[3].properties[3].type" Type="Str">Show horizontal scroll bar</Property>
				<Property Name="Source[3].properties[3].value" Type="Bool">true</Property>
				<Property Name="Source[3].properties[4].type" Type="Str">Show toolbar</Property>
				<Property Name="Source[3].properties[4].value" Type="Bool">false</Property>
				<Property Name="Source[3].properties[5].type" Type="Str">Show Abort button</Property>
				<Property Name="Source[3].properties[5].value" Type="Bool">true</Property>
				<Property Name="Source[3].properties[6].type" Type="Str">Show fp when called</Property>
				<Property Name="Source[3].properties[6].value" Type="Bool">false</Property>
				<Property Name="Source[3].properties[7].type" Type="Str">Window behavior</Property>
				<Property Name="Source[3].properties[7].value" Type="Str">Default</Property>
				<Property Name="Source[3].properties[8].type" Type="Str">Allow user to close window</Property>
				<Property Name="Source[3].properties[8].value" Type="Bool">true</Property>
				<Property Name="Source[3].properties[9].type" Type="Str">Window run-time position</Property>
				<Property Name="Source[3].properties[9].value" Type="Str">Centered</Property>
				<Property Name="Source[3].propertiesCount" Type="Int">10</Property>
				<Property Name="Source[3].type" Type="Str">VI</Property>
				<Property Name="Source[4].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[4].itemID" Type="Ref">/我的电脑/自校准.vi</Property>
				<Property Name="Source[4].type" Type="Str">VI</Property>
				<Property Name="SourceCount" Type="Int">5</Property>
				<Property Name="TgtF_companyName" Type="Str">lool</Property>
				<Property Name="TgtF_fileDescription" Type="Str">主程序</Property>
				<Property Name="TgtF_fileVersion.major" Type="Int">1</Property>
				<Property Name="TgtF_internalName" Type="Str">主程序</Property>
				<Property Name="TgtF_legalCopyright" Type="Str">版权 2016 lool</Property>
				<Property Name="TgtF_productName" Type="Str">主程序</Property>
				<Property Name="TgtF_targetfileGUID" Type="Str">{1CA663FA-073B-4CF1-925C-41DAF4D35297}</Property>
				<Property Name="TgtF_targetfileName" Type="Str">主程序.exe</Property>
			</Item>
			<Item Name="装置IP配置" Type="EXE">
				<Property Name="App_copyErrors" Type="Bool">true</Property>
				<Property Name="App_INI_aliasGUID" Type="Str">{2CD12F60-C523-4F7E-8243-58748D58388D}</Property>
				<Property Name="App_INI_GUID" Type="Str">{503C6D37-8402-436E-A074-20750B7A95DE}</Property>
				<Property Name="App_winsec.description" Type="Str">http://www.lool.com</Property>
				<Property Name="Bld_buildCacheID" Type="Str">{6CCA9A2C-1525-4429-B0A8-032682FF8815}</Property>
				<Property Name="Bld_buildSpecName" Type="Str">装置IP配置</Property>
				<Property Name="Bld_defaultLanguage" Type="Str">ChineseS</Property>
				<Property Name="Bld_excludeInlineSubVIs" Type="Bool">true</Property>
				<Property Name="Bld_excludeLibraryItems" Type="Bool">true</Property>
				<Property Name="Bld_excludePolymorphicVIs" Type="Bool">true</Property>
				<Property Name="Bld_localDestDir" Type="Path">../builds/装置IP配置</Property>
				<Property Name="Bld_localDestDirType" Type="Str">relativeToCommon</Property>
				<Property Name="Bld_modifyLibraryFile" Type="Bool">true</Property>
				<Property Name="Bld_previewCacheID" Type="Str">{3F572F48-B20F-4516-AD91-04032C4B49DE}</Property>
				<Property Name="Destination[0].destName" Type="Str">装置IP配置.exe</Property>
				<Property Name="Destination[0].path" Type="Path">../builds/装置IP配置/装置IP配置.exe</Property>
				<Property Name="Destination[0].preserveHierarchy" Type="Bool">true</Property>
				<Property Name="Destination[0].type" Type="Str">App</Property>
				<Property Name="Destination[1].destName" Type="Str">支持目录</Property>
				<Property Name="Destination[1].path" Type="Path">../builds/装置IP配置/data</Property>
				<Property Name="DestinationCount" Type="Int">2</Property>
				<Property Name="Source[0].itemID" Type="Str">{5EF75751-FBB0-4288-96C0-F690C7468C45}</Property>
				<Property Name="Source[0].type" Type="Str">Container</Property>
				<Property Name="Source[1].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[1].itemID" Type="Ref">/我的电脑/ipconfig.vi</Property>
				<Property Name="Source[1].sourceInclusion" Type="Str">TopLevel</Property>
				<Property Name="Source[1].type" Type="Str">VI</Property>
				<Property Name="SourceCount" Type="Int">2</Property>
				<Property Name="TgtF_companyName" Type="Str">lool</Property>
				<Property Name="TgtF_fileDescription" Type="Str">装置IP配置</Property>
				<Property Name="TgtF_fileVersion.major" Type="Int">1</Property>
				<Property Name="TgtF_internalName" Type="Str">装置IP配置</Property>
				<Property Name="TgtF_legalCopyright" Type="Str">版权 2016 lool</Property>
				<Property Name="TgtF_productName" Type="Str">装置IP配置</Property>
				<Property Name="TgtF_targetfileGUID" Type="Str">{2C11B221-A698-4EBA-8C82-A4D88BED4DF5}</Property>
				<Property Name="TgtF_targetfileName" Type="Str">装置IP配置.exe</Property>
			</Item>
			<Item Name="自校准" Type="EXE">
				<Property Name="App_copyErrors" Type="Bool">true</Property>
				<Property Name="App_INI_aliasGUID" Type="Str">{844F9D9A-C21F-40E2-BEDA-16337CDB6801}</Property>
				<Property Name="App_INI_GUID" Type="Str">{0C020AFA-991A-43DE-9554-D5BC2EC96B2B}</Property>
				<Property Name="Bld_buildCacheID" Type="Str">{218026BB-2217-426E-BAD2-FCE14669CAAC}</Property>
				<Property Name="Bld_buildSpecName" Type="Str">自校准</Property>
				<Property Name="Bld_defaultLanguage" Type="Str">ChineseS</Property>
				<Property Name="Bld_excludeInlineSubVIs" Type="Bool">true</Property>
				<Property Name="Bld_excludeLibraryItems" Type="Bool">true</Property>
				<Property Name="Bld_excludePolymorphicVIs" Type="Bool">true</Property>
				<Property Name="Bld_localDestDir" Type="Path">../builds/自校准</Property>
				<Property Name="Bld_localDestDirType" Type="Str">relativeToCommon</Property>
				<Property Name="Bld_modifyLibraryFile" Type="Bool">true</Property>
				<Property Name="Bld_previewCacheID" Type="Str">{51B00601-C395-4071-859D-D9AEEBD62788}</Property>
				<Property Name="Destination[0].destName" Type="Str">自校准.exe</Property>
				<Property Name="Destination[0].path" Type="Path">../builds/自校准/自校准.exe</Property>
				<Property Name="Destination[0].preserveHierarchy" Type="Bool">true</Property>
				<Property Name="Destination[0].type" Type="Str">App</Property>
				<Property Name="Destination[1].destName" Type="Str">支持目录</Property>
				<Property Name="Destination[1].path" Type="Path">../builds/自校准/data</Property>
				<Property Name="DestinationCount" Type="Int">2</Property>
				<Property Name="Source[0].itemID" Type="Str">{5EF75751-FBB0-4288-96C0-F690C7468C45}</Property>
				<Property Name="Source[0].type" Type="Str">Container</Property>
				<Property Name="Source[1].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[1].itemID" Type="Ref">/我的电脑/自校准.vi</Property>
				<Property Name="Source[1].sourceInclusion" Type="Str">TopLevel</Property>
				<Property Name="Source[1].type" Type="Str">VI</Property>
				<Property Name="SourceCount" Type="Int">2</Property>
				<Property Name="TgtF_companyName" Type="Str">lool</Property>
				<Property Name="TgtF_fileDescription" Type="Str">自校准</Property>
				<Property Name="TgtF_fileVersion.major" Type="Int">1</Property>
				<Property Name="TgtF_internalName" Type="Str">自校准</Property>
				<Property Name="TgtF_legalCopyright" Type="Str">版权 2016 lool</Property>
				<Property Name="TgtF_productName" Type="Str">自校准</Property>
				<Property Name="TgtF_targetfileGUID" Type="Str">{5E695CF2-F901-4ABC-A479-1E930EE62460}</Property>
				<Property Name="TgtF_targetfileName" Type="Str">自校准.exe</Property>
			</Item>
		</Item>
	</Item>
</Project>
