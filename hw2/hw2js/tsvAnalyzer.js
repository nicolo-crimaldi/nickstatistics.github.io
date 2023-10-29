
function evalFrequency(){
//initializing objects to calculate absolute frequency
    var ageFrequency = {};
    var backgroundFrequency = {};
    var heightFrequency = {};
    //actual frequency calculation
    matrix.forEach((column)=>{
        for (let row = 0; row < matrix.length; row++) {//first for is for age frequency
            if(matrix[0][column] === document.querySelector(exampleInputEmail1).value){
                let age = matrix[row][column];
                ageFrequency += (age || 0) + 1;
            }
        }
        for (let row = 0; row < matrix.length; row++) {//second for is for height frequency
            if(matrix[0][column] === document.querySelector(exampleInputPassword1).value){
                let height = parseFloat(matrix[row][column]);
                heightFrequency += (height || 0) + 1;
                }
        }
        for (let row = 0; row < matrix.length; row++) {//third for is for background frequency
            if(matrix[0][column] === document.querySelector(exampleInputVariable1).value){
                let background = parseFloat(matrix[row][column]);
                backgroundFrequency += (background || 0) + 1;
            }
        }
    });
    displayTable(ageFrequency, document.querySelector(exampleInputEmail1));
    displayTable(backgroundFrequency, document.querySelector(exampleInputPassword1));
    displayTable(heightFrequency, document.querySelector(exampleInputVariable1).value);
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