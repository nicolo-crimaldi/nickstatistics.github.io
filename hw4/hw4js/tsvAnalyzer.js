
function evalFrequency(variableArray, nIntervals){
    const max = Math.max(...variableArray);
    const min = Math.min(...variableArray);
    const span = (max - min) / nIntervals;//we establish the radius for a single interval
    const absoluteFrequency[nIntervals] = [];//this array contains the frequency distribution for every interval of the variable selected

    for (let i = 0; i < absoluteFrequency.length; i++) {
        variableArray.forEach((element) => {
            if ((element >= min * i && element <= (min * i) + span) || element >= max) {//check if the current element is in the current interval
                absoluteFrequency[i] = (absoluteFrequency[i] || 0) + 1;//update the frequency using the or with 0
            }
        });
    }

    return absoluteFrequency;
}

function relativeFrequency(variableArray, nIntervals){
    let relativeFrequency = [];
    let absoluteFrequency = evalFrequency(variableArray, nIntervals); 
    for (let i = 0; i < absoluteFrequency.length; i++) {
        relativeFrequency.forEach((element) => {
            element = absoluteFrequency[i]/absoluteFrequency.length;
        });
    }
    return relativeFrequency;
}

function percentageFrequency(variableArray, nIntervals){
    let percentageFrequency = [];
    let relativeFrequency = relativeFrequency(variableArray, nIntervals);
    for (let i = 0; i < relativeFrequency.length; i++) {
        percentageFrequency.forEach((element) => {
            element = relativeFrequency[i] * 100;
        });        
    }
    return percentageFrequency;
}
                    

function displayTable(frequencies, variable) {
    var table = '<h3>' + variable + '</h3>';
    table += '<table>';
    table += '<tr><th>Value</th><th>Absolute Frequency</th><th>Relative Frequency</th><th>Percentage</th></tr>';

    var total = Object.values(frequencies).reduce((acc, val) => acc + val, 0);

    Object.entries(frequencies).forEach(function ([key, value]) {
        var relativeFrequency = value / total;
        var percentage = (relativeFrequency * 100).toFixed(2) + '%';

        table += '<tr><td>' + key + '</td><td>' + value + '</td><td>' + relativeFrequency.toFixed(2) + '</td><td>' + percentage + '</td></tr>';
    });

    table += '</table>';
    document.body.innerHTML += table;
}

function calculateFrequencies(input = matrix) {
    // Initialize objects for frequencies
    var combinedFrequency = {};

    // Calculate frequencies for combined
    input.forEach(function (row) {
        var combined = row.join(' - ');
        combinedFrequency[combined] = (combinedFrequency[combined] || 0) + 1;
    });

    // Display the results in a table
    displayTableJ(combinedFrequency, 'Combined (Background - Age)');
}


function displayTableJ(frequencies, variable) {
    var table = '<h3>' + variable + '</h3>';
    table += '<table>';
    table += '<tr><th>Value</th><th>Frequency</th></tr>';

    Object.entries(frequencies).forEach(function ([key, value]) {
        table += '<tr><td>' + key + '</td><td>' + value + '</td></tr>';
    });

    table += '</table>';
    document.getElementById('resultTables').innerHTML = table;
}