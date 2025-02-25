document.addEventListener('DOMContentLoaded', function () {
    const studyTable = document.getElementById('studyTable');

    if (studyTable) {
        studyTable.addEventListener('click', function (e) {
            const courseRow = e.target.closest('.course-row');
            if (courseRow) {
                e.preventDefault();
                const courseId = courseRow.getAttribute('data-course-id');
                fetch('/Groups/ByCourse/' + courseId)
                    .then(function (response) {
                        if (!response.ok) {
                            throw new Error(response.message);
                        }
                        return response.text();
                    })
                    .then(function (data) {
                        studyTable.setAttribute('data-course-id', courseId);
                        studyTable.innerHTML = data;
                    })
                    .catch(function (error) {
                        alert(error.message);
                    });
                return;
            }

            const groupRow = e.target.closest('.group-row');
            if (groupRow) {
                e.preventDefault();
                const groupId = groupRow.getAttribute('data-group-id');
                fetch('/Students/ByGroup/' + groupId)
                    .then(function (response) {
                        if (!response.ok) {
                            throw new Error(response.message);
                        }
                        return response.text();
                    })
                    .then(function (data) {
                        studyTable.innerHTML = data;
                    })
                    .catch(function (error) {
                        alert(error.message);
                    });
                return;
            }
        });
    }

    document.addEventListener('click', function (e) {
        if (e.target && e.target.id === 'backToCourses') {
            e.preventDefault();
            fetch('/Courses/CoursesPartial')
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error(response.message);
                    }
                    return response.text();
                })
                .then(function (data) {
                    document.getElementById('studyTable').innerHTML = data;
                })
                .catch(function (error) {
                    alert(error.message);
                });
        }
    });

    document.addEventListener('click', function (e) {
        if (e.target && e.target.id === 'backToGroups') {
            e.preventDefault();
            var courseId = document.getElementById('studyTable').getAttribute('data-course-id');
            if (courseId) {
                fetch('/Groups/ByCourse/' + courseId)
                    .then(function (response) {
                        if (!response.ok) {
                            throw new Error(response.message);
                        }
                        return response.text();
                    })
                    .then(function (data) {
                        document.getElementById('studyTable').innerHTML = data;
                    })
                    .catch(function (error) {
                        alert(error.message);
                    });
            } else {
                alert('Course ID not found.');
            }
        }
    });
});
