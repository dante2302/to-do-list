export const getNearest10MinuteInterval = () => 
{
    return roundToNext10Minutes(new Date());
}

const roundToNext10Minutes = (date: Date): Date => {
  const minutes = date.getMinutes();
  const remainder = minutes % 10;
  
  // If the current time is exactly on a 10-minute boundary (e.g., 19:20), move to the next interval
  const roundedMinutes = remainder === 0 ? minutes + 10 : minutes + (10 - remainder);
  
  date.setMinutes(roundedMinutes, 0, 0); // Set the seconds and milliseconds to 0
  return date;
};