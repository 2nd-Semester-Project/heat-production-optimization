using Xunit;

namespace HeatOptimiser.Tests {
    public class ResultsDataManagerTests
    {
        private ResultsDataManager manager;

        public ResultsDataManagerTests()
        {
            manager = new ResultsDataManager();
        }

        [Fact]
        public void Add_Schedule_SuccessfullyAddsSchedule()
        {
            // Arrange
            var startDate = DateTime.Now;
            var endDate = startDate.AddDays(1);
            var schedule = new Schedule(startDate, endDate); // Directly instantiate Schedule

            // Act
            manager.Add(schedule);

            // Assert
            Assert.Contains(schedule, manager.Schedules); // Assuming there's a way to access schedules
        }

        [Fact]
        public void Edit_Schedule_RecalculatesAndUpdatesSchedule()
        {
            // Arrange
            var startDate = DateTime.Now;
            var endDate = startDate.AddDays(1);
            var originalSchedule = new Schedule(startDate, endDate);
            manager.Add(originalSchedule);
            
            var newEndDate = endDate.AddDays(2);
            var updatedSchedule = new Schedule(startDate, newEndDate); // This represents the updated details
            
            // Act
            manager.Edit(originalSchedule, updatedSchedule); // Assuming Edit updates the schedule by use of Optimiser

            // Assert
            Assert.DoesNotContain(originalSchedule, manager.Schedules);
            var existingSchedule = manager.Schedules.FirstOrDefault(s => s.startDate == startDate && s.endDate == newEndDate);
            Assert.NotNull(existingSchedule); // Verify the updated schedule exists
        }

        [Fact]
        public void Remove_ScheduleWithoutDate_RemovesAllSchedules()
        {
            // Arrange
            var scheduleOne = new Schedule(DateTime.Now, DateTime.Now.AddDays(1));
            var scheduleTwo = new Schedule(DateTime.Now.AddDays(2), DateTime.Now.AddDays(3));
            manager.Add(scheduleOne);
            manager.Add(scheduleTwo);

            // Act
            manager.Remove(); // Assuming Remove() without parameters clears all schedules

            // Assert
            Assert.Empty(manager.Schedules);
        }

        [Fact]
        public void Remove_ScheduleWithDateRange_RemovesSchedulesWithinRange()
        {
            // Arrange
            var scheduleOne = new Schedule(DateTime.Now, DateTime.Now.AddDays(1));
            var scheduleTwo = new Schedule(DateTime.Now.AddDays(2), DateTime.Now.AddDays(3));
            var scheduleThree = new Schedule(DateTime.Now.AddDays(4), DateTime.Now.AddDays(5));
            manager.Add(scheduleOne);
            manager.Add(scheduleTwo);
            manager.Add(scheduleThree);
            
            // Act
            manager.Remove(DateTime.Now.AddDays(1), DateTime.Now.AddDays(4)); // Removes schedules overlapping with this range

            // Assert
            Assert.DoesNotContain(scheduleOne, manager.Schedules);
            Assert.DoesNotContain(scheduleTwo, manager.Schedules);
            Assert.Contains(scheduleThree, manager.Schedules); // Only scheduleThree should remain
        }
    }
}