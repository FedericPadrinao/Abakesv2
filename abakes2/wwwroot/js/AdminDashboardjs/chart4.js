document.addEventListener("DOMContentLoaded", function () {
    var chartElement = document.getElementById('doughnut3');
    var url = chartElement.dataset.url;

    fetch(url)
        .then(response => response.json())
        .then(data => {
            var colors = [
                'rgba(255, 165, 0, 1)',
                'rgba(0, 128, 0, 1)',
                'rgba(54, 162, 235, 1)',
                'rgba(128, 128, 128, 1)',
                'rgba(0, 0, 128, 1)',
                'rgba(0, 0, 255, 1)',
                'rgba(255, 0, 0, 1)',
                'rgba(255, 206, 86, 1)',
                'rgba(255, 255, 0, 1)',
                'rgba(75, 192, 192, 1)',
                'rgba(255, 99, 132, 1)',
                'rgba(120, 46, 139, 1)',
                'rgba(0, 255, 255, 1)',
                'rgba(0, 0, 0, 1)',
                'rgba(41, 155, 99, 1)',
                'rgba(128, 0, 0, 1)',
                'rgba(128, 0, 0, 1)',
                'rgba(0, 128, 128, 1)',
                'rgba(0, 0, 0, 1)'
            ];
            var ctx2 = chartElement.getContext('2d');
            var myChart2 = new Chart(ctx2, {
                type: 'doughnut',
                data: {
                    labels: data.labels,
                    datasets: [{
                        label: 'Cake Colors',
                        data: data.data,
                        backgroundColor: colors,
                        borderColor: colors,
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true
                }
            });
        })
        .catch(error => console.error('Error fetching data:', error));
});
