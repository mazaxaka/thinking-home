﻿define(['app', 'lib',
		'webapp/alarm-clock/editor-model',
		'webapp/alarm-clock/editor-view'],
	function (application, lib, models, views) {

		var api = {
			createEditor: function (model) {

				var view = new views.AlarmEditorView({ model: model });

				view.on('alarm-clock:editor:save', api.save);
				view.on('alarm-clock:editor:cancel', api.redirectToList);
				view.on('alarm-clock:editor:delete', api.deleteAlarm);

				application.setContentView(view);
			},

			edit: function (id) {

				models.loadEditorData(id).done(api.createEditor);
			},

			save: function () {

				var model = this.model.toJSON();
				models.saveAlarm(model).done(api.redirectToList);
			},
			deleteAlarm: function() {
				
				var name = this.model.get('name');

				if (lib.utils.confirm('Do you want to delete the alarm?', name)) {

					var id = this.model.get('id');
					models.deleteAlarm(id).done(api.redirectToList);
				}
			},
			redirectToList: function () {
				application.navigate('webapp/alarm-clock/list');
			}
		};

		return {
			start: function (id) {
				api.edit(id);
			}
		};
	});