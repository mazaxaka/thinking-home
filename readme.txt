﻿ПЛАН
- настройки плагинов



ИНТЕРФЕЙС
	организация
		- рабочий стол (панель с тайлами);
		- список общих разделов;
		- список системных разделов
	
	настройка
		- можно настраивать наборы тайлов (элементы навигационного меню и тайлы указывают на методы api)

	средства для разработчика
		- возможность добавлять элементы в список доступных разделов
		- возможность добавлять элементы в список доступных тайлов
		- возможность посылать оповещения на клиент (обновление тайлов или интерфейса)
		- возможность обработки оповещений от сервиса

	описание логики работы
		- инфраструктуру для виджетов предоставляет специальный плагин WebUI
		- разделы (sections) - небольшие модули marionette.js, физически файлы лежат в ресурсах плагина
		- содержимое рабочего стола (коллекция плиток) грузится одним запросом (только данные)
		- разделы добавляются в списки общих и системных разделов при помощи серверных атрибутов
		


ПРАВИЛА ИМЕНОВАНИЯ
- URL
	/api/{pluginAlias}/{methodAlias}	- методы API
	/webapp/{pluginAlias}/{filePath}	- ресурсы

МОДУЛИ
	{PluginAlias}.{ModuleAlias}

СОБЫТИЯ ПРЕДСТАВЛЕНИЙ
	this.trigger('{pluginAlias}:{eventName}');

ЗАПРОСЫ
	app.request('{requestType}:{pluginAlias}:{requestName}')
	{requestType}:	get	: без асинхронного обращения к серверу
			load	: асинхронное обращение к серверу для получения данных
			update	: асинхронное обращение к серверу для отправки данных


организация проекта
	компиляция плагинов
		..\build\Debug\<название.плагина>
		ставим всем элементам References свойство Copy Local = false


плагины для реализации:
	- будильник
	- погода
	- хранилище


hourly -    http://api.wunderground.com/api/6924685d839dcbf6/hourly/lang:RU/q/Russia/Moscow.json
forecast -  http://api.wunderground.com/api/6924685d839dcbf6/forecast/lang:RU/q/Russia/Moscow.json
