/**
 * Form Picker
 */

'use strict';

(function () {
  // Flat Picker
  // --------------------------------------------------------------------

  // Helper: add "Today" button to calendar footer
  function addTodayBtn(selectedDates, dateStr, instance) {
    var container = instance.calendarContainer;
    if (!container) return;
    if (container.querySelector('.fp-today-btn')) return;

    var todayBtn = document.createElement('button');
    todayBtn.type = 'button';
    todayBtn.className = 'fp-today-btn btn btn-sm btn-outline-primary w-100 mt-2';
    todayBtn.textContent = 'Today';
    todayBtn.addEventListener('click', function () {
      instance.setDate(new Date(), true);
      instance.close();
    });
    container.appendChild(todayBtn);
  }

  // Date
  flatpickr('.flatpickr-date', {
    monthSelectorType: 'static',
    dateFormat: 'd-M-Y',
    static: true,
    onReady: addTodayBtn
  });

  // Time
  flatpickr('.flatpickr-time', {
    enableTime: true,
    noCalendar: true,
    static: true
  });

  // Datetime
  flatpickr('.flatpickr-datetime', {
    enableTime: true,
    altInput: true,
    altFormat: 'd-M-Y H:i',
    dateFormat: 'd-M-Y H:i',
    static: true,
    onReady: addTodayBtn
  });

  // Multi Date Select
  flatpickr('.flatpickr-multi', {
    altFormat: 'd-M-Y',
    dateFormat: 'd-M-Y',
    weekNumbers: true,
    enableTime: true,
    mode: 'multiple',
    minDate: 'today',
    static: true,
    onReady: addTodayBtn
  });

  // Range
  flatpickr('.flatpickr-range', {
    mode: 'range',
    altInput: true,
    altFormat: 'd-M-Y',
    dateFormat: 'd-M-Y',
    static: true,
    onReady: addTodayBtn
  });

  // Inline
  flatpickr('.flatpickr-inline', {
    inline: true,
    allowInput: false,
    dateFormat: 'd-M-Y',
    monthSelectorType: 'static',
    onReady: addTodayBtn
  });

  // Human Friendly
  flatpickr('.flatpickr-human-friendly', {
    altInput: true,
    altFormat: 'd-M-Y',
    dateFormat: 'd-M-Y',
    static: true,
    onReady: addTodayBtn
  });

  // Friendly (Components page)
  document.querySelectorAll('.flatpickr-friendly').forEach(function (el) {
    el.flatpickr({
      altInput: true,
      altFormat: 'd-M-Y',
      dateFormat: 'd-M-Y',
      static: true,
      onReady: addTodayBtn
    });
  });

  // Disabled Date Range
  const fromDate = new Date(Date.now() - 3600 * 1000 * 48);
  const toDate = new Date(Date.now() + 3600 * 1000 * 48);

  flatpickr('.flatpickr-disabled-range', {
    dateFormat: 'd-M-Y',
    disable: [
      {
        from: fromDate.toISOString().split('T')[0],
        to: toDate.toISOString().split('T')[0]
      }
    ],
    static: true,
    onReady: addTodayBtn
  });
})();

// Global helper: set flatpickr date from any format
function setFlatpickrDate(selector, dateStr) {
  if (!dateStr || dateStr === '01 Jan 1900' || dateStr === '0001-01-01T00:00:00') return;
  var d = new Date(dateStr);
  if (isNaN(d.getTime())) return;
  var months = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'];
  var formatted = String(d.getDate()).padStart(2,'0') + '-' + months[d.getMonth()] + '-' + d.getFullYear();
  var el = document.querySelector(selector);
  if (el && el._flatpickr) {
    el._flatpickr.setDate(formatted, true);
  } else {
    $(selector).val(formatted);
  }
}

// * Pickers with jQuery dependency (jquery)
$(function () {
  // Bootstrap Daterange Picker
  // --------------------------------------------------------------------
  var bsRangePickerBasic = $('#bs-rangepicker-basic'),
    bsRangePickerSingle = $('#bs-rangepicker-single'),
    bsRangePickerTime = $('#bs-rangepicker-time'),
    bsRangePickerRange = $('#bs-rangepicker-range'),
    bsRangePickerWeekNum = $('#bs-rangepicker-week-num'),
    bsRangePickerDropdown = $('#bs-rangepicker-dropdown');

  // Basic
  if (bsRangePickerBasic.length) {
    bsRangePickerBasic.daterangepicker({
      opens: isRtl ? 'left' : 'right'
    });
  }

  // Single
  if (bsRangePickerSingle.length) {
    bsRangePickerSingle.daterangepicker({
      singleDatePicker: true,
      opens: isRtl ? 'left' : 'right'
    });
  }

  // Time & Date
  if (bsRangePickerTime.length) {
    bsRangePickerTime.daterangepicker({
      timePicker: true,
      timePickerIncrement: 30,
      locale: {
        format: 'MM/DD/YYYY h:mm A'
      },
      opens: isRtl ? 'left' : 'right'
    });
  }

  if (bsRangePickerRange.length) {
    bsRangePickerRange.daterangepicker({
      ranges: {
        Today: [moment(), moment()],
        Yesterday: [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
        'Last 7 Days': [moment().subtract(6, 'days'), moment()],
        'Last 30 Days': [moment().subtract(29, 'days'), moment()],
        'This Month': [moment().startOf('month'), moment().endOf('month')],
        'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
      },
      opens: isRtl ? 'left' : 'right'
    });
  }

  // Week Numbers
  if (bsRangePickerWeekNum.length) {
    bsRangePickerWeekNum.daterangepicker({
      showWeekNumbers: true,
      opens: isRtl ? 'left' : 'right'
    });
  }
  // Dropdown
  if (bsRangePickerDropdown.length) {
    bsRangePickerDropdown.daterangepicker({
      showDropdowns: true,
      opens: isRtl ? 'left' : 'right'
    });
  }

  const bsRangePickerCancelBtn = document.getElementsByClassName('cancelBtn');

  // Adding btn-label-primary class and removing btn-default in cancel buttons
  Array.from(bsRangePickerCancelBtn).forEach(btn => {
    btn.classList.remove('btn-default');
    btn.classList.add('btn-label-primary');
  });

  // jQuery Timepicker
  // --------------------------------------------------------------------
  var basicTimepicker = $('#timepicker-basic'),
    minMaxTimepicker = $('#timepicker-min-max'),
    disabledTimepicker = $('#timepicker-disabled-times'),
    formatTimepicker = $('#timepicker-format'),
    stepTimepicker = $('#timepicker-step'),
    altHourTimepicker = $('#timepicker-24hours');

  // Basic
  if (basicTimepicker.length) {
    basicTimepicker.timepicker({
      orientation: isRtl ? 'r' : 'l'
    });
  }

  // Min & Max
  if (minMaxTimepicker.length) {
    minMaxTimepicker.timepicker({
      minTime: '2:00pm',
      maxTime: '7:00pm',
      showDuration: true,
      orientation: isRtl ? 'r' : 'l'
    });
  }

  // Disabled Picker
  if (disabledTimepicker.length) {
    disabledTimepicker.timepicker({
      disableTimeRanges: [
        ['12am', '3am'],
        ['4am', '4:30am']
      ],
      orientation: isRtl ? 'r' : 'l'
    });
  }

  // Format Picker
  if (formatTimepicker.length) {
    formatTimepicker.timepicker({
      timeFormat: 'H:i:s',
      orientation: isRtl ? 'r' : 'l'
    });
  }

  // Steps Picker
  if (stepTimepicker.length) {
    stepTimepicker.timepicker({
      step: 15,
      orientation: isRtl ? 'r' : 'l'
    });
  }

  // 24 Hours Format
  if (altHourTimepicker.length) {
    altHourTimepicker.timepicker({
      show: '24:00',
      timeFormat: 'H:i:s',
      orientation: isRtl ? 'r' : 'l'
    });
  }
});

// color picker (pickr)
// --------------------------------------------------------------------
(function () {
  const classicPicker = document.querySelector('#color-picker-classic'),
    monolithPicker = document.querySelector('#color-picker-monolith'),
    nanoPicker = document.querySelector('#color-picker-nano');

  // classic
  if (classicPicker) {
    const classicPickr = new Pickr({
      el: classicPicker,
      theme: 'classic',
      default: 'rgba(102, 108, 232, 1)',
      swatches: [
        'rgba(102, 108, 232, 1)',
        'rgba(40, 208, 148, 1)',
        'rgba(255, 73, 97, 1)',
        'rgba(255, 145, 73, 1)',
        'rgba(30, 159, 242, 1)'
      ],
      components: {
        // Main components
        preview: true,
        opacity: true,
        hue: true,

        // Input / output Options
        interaction: {
          hex: true,
          rgba: true,
          hsla: true,
          hsva: true,
          cmyk: true,
          input: true,
          clear: true,
          save: true
        }
      }
    });
  }

  // monolith
  if (monolithPicker) {
    const monoPickr = new Pickr({
      el: monolithPicker,
      theme: 'monolith',
      default: 'rgba(40, 208, 148, 1)',
      swatches: [
        'rgba(102, 108, 232, 1)',
        'rgba(40, 208, 148, 1)',
        'rgba(255, 73, 97, 1)',
        'rgba(255, 145, 73, 1)',
        'rgba(30, 159, 242, 1)'
      ],
      components: {
        // Main components
        preview: true,
        opacity: true,
        hue: true,

        // Input / output Options
        interaction: {
          hex: true,
          rgba: true,
          hsla: true,
          hsva: true,
          cmyk: true,
          input: true,
          clear: true,
          save: true
        }
      }
    });
  }

  // nano
  if (nanoPicker) {
    const nanoPickr = new Pickr({
      el: nanoPicker,
      theme: 'nano',
      default: 'rgba(255, 73, 97, 1)',
      swatches: [
        'rgba(102, 108, 232, 1)',
        'rgba(40, 208, 148, 1)',
        'rgba(255, 73, 97, 1)',
        'rgba(255, 145, 73, 1)',
        'rgba(30, 159, 242, 1)'
      ],
      components: {
        // Main components
        preview: true,
        opacity: true,
        hue: true,

        // Input / output Options
        interaction: {
          hex: true,
          rgba: true,
          hsla: true,
          hsva: true,
          cmyk: true,
          input: true,
          clear: true,
          save: true
        }
      }
    });
  }
})();
