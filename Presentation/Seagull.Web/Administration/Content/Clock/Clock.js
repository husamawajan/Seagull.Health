//Clock-Husam Awajan
$(function () {

    var clock = $("#clock"),
      alarm = clock.find('.alarm'),
      ampm = clock.find('.ampm');

    var digitsToName = "zero one two three four five six seven eight nine".split(" ");

    var digits = {};

    var positions = ['h1', 'h2', ':', 'm1', 'm2', ':', 's1', 's2'];

    var digitHolder = clock.find('.digits');

    $.each(positions, function () {

        if (this == ":") {
            digitHolder.append('<div class="dots blink">');
        }
        else {
            var pos = $('<div>');

            for (var i = 0; i < 7; i++) {
                pos.append('<span class="d' + (i + 1) + '">');
            }

            digits[this] = pos;

            digitHolder.append(pos);
        }

    });


    var dayNames = "SUN MON TUE WED THU FRI SAT".split(' '),
      dayHolder = clock.find('.weekdays');


    $.each(dayNames, function () {
        dayHolder.append('<span>' + this + '</span>');
    });

    var weekdays = clock.find('.weekdays span');


    (function updateTime() {

        var currTime = moment().format("hhmmssdA");

        digits.h1.attr('class', digitsToName[currTime[0]]);
        digits.h2.attr('class', digitsToName[currTime[1]]);
        digits.m1.attr('class', digitsToName[currTime[2]]);
        digits.m2.attr('class', digitsToName[currTime[3]]);
        digits.s1.attr('class', digitsToName[currTime[4]]);
        digits.s2.attr('class', digitsToName[currTime[5]]);

        var day = currTime[6];


        weekdays.removeClass('active').eq(day).addClass('active');


        ampm.text(currTime[7] + currTime[8]);

        setTimeout(updateTime, 1000);
    })();


    $('.btn').click(function () {
        //clock.toggleClass('light dark');
    });
});