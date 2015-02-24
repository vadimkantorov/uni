data <- read.table("D:\\__STUDY__\\4 курс\\ВЭ\\Lab2\\output.txt", header=TRUE)
plot_colors <- c("blue", "red", "cyan", "forestgreen", "magenta", "gray")

par(cex=0.4)
plot(data$X, data$I, ylim=c(0,3), xlim=c(0,1), col=plot_colors[1], ann=FALSE, type="o", axes=FALSE, panel.first = grid())
lines(data$X, data$T, col=plot_colors[2], type="o")
lines(data$X, data$E, col=plot_colors[3], type="o")
lines(data$X, data$S3, col=plot_colors[4], type="o")
lines(data$X, data$S5, col=plot_colors[5], type="o")
lines(data$X, data$K2, col=plot_colors[6], type="o")
par(cex=1)

axis(1, at=seq(0,1,by=0.1))
axis(2, at=seq(0,3,by=0.1))


title(main="Solving ODEs", font.main=4)
title(xlab="X")
title(ylab="Y")

box()
legend("topleft", c(
"Analytical solution",
"Taylor 2nd order",
"Euler explicit",
"Simpson's implicit 3rd order",
"Simpson's implicit 5th order",
"Simpson's K2"
), cex=0.8, col=plot_colors, lwd=2, bty="n");