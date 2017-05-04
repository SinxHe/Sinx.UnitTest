### API调用
1. API架构风格
	* 本API采用RESTful架构风格, 关于这方面的知识可以参阅<<RESTful Web Services Cookbook>>
1. 版本控制
	* 默认使用最原始版本
	* 您可以在URL连接中添加参数ApiVersion=1.0来进行版本选择
	* 您也可以在报文行中添加ApiVersion: 1.1来进行版本选择
	* 以上两种版本干预方式都使用以URL中的版本为准

------------
### API列表和调用测试
	请直接访问: http://api.example.com/flumeQuery